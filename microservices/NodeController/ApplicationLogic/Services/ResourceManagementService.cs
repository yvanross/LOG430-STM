using System.Collections.Immutable;
using ApplicationLogic.Interfaces;
using Entities.BusinessObjects.States;
using Entities.DomainInterfaces.Live;
using Entities.BusinessObjects.Live;
using Entities.Dao;
using Entities.DomainInterfaces.Planned;
using Entities.Extensions;

namespace ApplicationLogic.Services;

public class ResourceManagementService
{
    private readonly IEnvironmentClient _environmentClient;

    private readonly IPodReadService _podReadService;
    
    private readonly IPodWriteService _writeServiceService;

    private readonly IHostInfo _hostInfo;

    public ResourceManagementService(IEnvironmentClient environmentClient, IPodReadService podReadService, IPodWriteService writeServiceService, IHostInfo hostInfo)
    {
        _environmentClient = environmentClient;
        _podReadService = podReadService;
        _writeServiceService = writeServiceService;
        _hostInfo = hostInfo;
    }

    public async Task RemovePodInstance(IPodInstance podInstance, bool soft = false)
    {
        foreach (var serviceInstance in podInstance.ServiceInstances)
        {
            if(serviceInstance.ContainerInfo is not null) 
                await _environmentClient.RemoveContainerInstance(serviceInstance.ContainerInfo.Id, soft: soft);
        }

        _writeServiceService.TryRemovePod(podInstance);
    }

    public async Task ReplacePodInstance(IPodInstance podInstance)
    {
        await RemovePodInstance(podInstance);

        try
        {
            foreach (var serviceInstance in podInstance.ServiceInstances)
            {
                serviceInstance.ServiceStatus = new LaunchedState();

                var serviceType = _podReadService.GetServiceType(serviceInstance.Type);

                if (serviceType is not null && _podReadService.GetPodType(podInstance.Type) is {} podType)
                {
                    var newContainerName = GetNewContainerName(serviceType, podType, serviceInstance);

                    var creationId = await _environmentClient.IncreaseByOneNumberOfInstances(serviceType.ContainerConfig, newContainerName, serviceInstance, podType);

                    var containerInfo = await _environmentClient.GetContainerInfo(creationId);

                    serviceInstance.ContainerInfo = containerInfo.CuratedInfo;
                }
            }
            _writeServiceService.AddOrUpdatePod(podInstance);
        }
        catch (Exception e)
        {
            await RemovePodInstance(podInstance);

            throw new Exception($"{nameof(ReplacePodInstance)} {e}");
        }
    }

    public async Task CreateNewSidecarInstanceOnPod(string serviceTypeName, string podTypeName, string podId)
    {
        var podType = _podReadService.GetPodType(podTypeName);

        var serviceType = _podReadService.GetServiceType(serviceTypeName);

        var podInstance = _podReadService.GetPodById(podId);

        if (podType is not null && serviceType is not null && podInstance is not null)
        {
            var newServiceId = Guid.NewGuid().ToString();

            var newServiceInstance = new ServiceInstance()
            {
                Address = _hostInfo.GetAddress(),
                Id = newServiceId,
                ContainerInfo = default,
                Type = serviceType.Type,
                PodId = podId,
                ServiceStatus = new LaunchedState(),
            };

            try
            {
                if (serviceType.Type.Equals(podType.Type) is false)
                    podTypeName = podType.Type + ".";

                var newContainerName = $"{podTypeName}{newServiceInstance!.Type}-{Random.Shared.Next(0, int.MaxValue)}";

                var creationInfo = await _environmentClient.IncreaseByOneNumberOfInstances(serviceType.ContainerConfig, newContainerName, newServiceInstance, podType);

                var containerInfo = await _environmentClient.GetContainerInfo(creationInfo);

                newServiceInstance.ContainerInfo = containerInfo.CuratedInfo;

                podInstance.AddServiceInstance(newServiceInstance);

                _writeServiceService.AddOrUpdatePod(podInstance);
            }
            catch (Exception e)
            {
                throw new Exception($"{nameof(CreateNewSidecarInstanceOnPod)} {e}");
            }
        }
    }

    public async Task IncreaseNumberOfPodInstances(string type)
    {
        var podType = _podReadService.GetPodType(type);

        if (podType is not null)
        {
            var newPodInstance = CreateNewPodInstance();

            try
            {
                foreach (var newServiceInstance in newPodInstance.ServiceInstances)
                {
                    newServiceInstance.ServiceStatus = new LaunchedState();

                    var serviceType = _podReadService.GetServiceType(newServiceInstance.Type);

                    if (serviceType is not null)
                    {
                        var newContainerName = GetNewContainerName(serviceType, podType, newServiceInstance);

                        var creationInfo = await _environmentClient.IncreaseByOneNumberOfInstances(serviceType.ContainerConfig, newContainerName, newServiceInstance, podType);

                        var containerInfo = await _environmentClient.GetContainerInfo(creationInfo);

                        newServiceInstance.ContainerInfo = containerInfo.CuratedInfo;
                    }
                }

                _writeServiceService.AddOrUpdatePod(newPodInstance);
            }
            catch (Exception e)
            {
                await RemovePodInstance(newPodInstance);

                throw new Exception($"{nameof(IncreaseNumberOfPodInstances)} {e}");
            }
        }

        PodInstance CreateNewPodInstance()
        {
            var serviceInstances = new List<IServiceInstance>();

            var newPodId = Guid.NewGuid().ToString();

            foreach (var serviceType in podType.ServiceTypes)
            {
                var newServiceId = Guid.NewGuid().ToString();

                serviceInstances.Add(new ServiceInstance()
                {
                    Address = _hostInfo.GetAddress(),
                    Id = newServiceId,
                    ContainerInfo = default,
                    Type = serviceType.Type,
                    PodId = newPodId,
                    ServiceStatus = new LaunchedState(),
                });
            }

            var newPodInstance = new PodInstance(_podReadService)
            {
                Id = newPodId,
                ServiceInstances = serviceInstances.ToImmutableList(),
                Type = podType.Type,
                ServiceStatus = new LaunchedState()
            };

            return newPodInstance;
        }
    }

    private string GetNewContainerName(IServiceType serviceType, IPodType podType, IServiceInstance newServiceInstance)
    {
        var podTypeName = string.Empty;
        var namesNotYetTaken = Enumerable.Empty<string>();

        if (serviceType.Type.EqualsIgnoreCase(podType.Type) is false)
            podTypeName = podType.Type + ".";

        if (serviceType.Type.EqualsIgnoreCase(podType.PodLeader?.Type))
            namesNotYetTaken = podType.ReplicasHostnames.WhereNotTrueInAnyOf(_podReadService.GetServiceInstances(podType.PodLeader.Type),
                (containerName, instance) =>
                    instance.ContainerInfo!.Name.Split(".").Last().EqualsIgnoreCase(containerName));

        var newContainerName = namesNotYetTaken.Any()
            ? $"{podTypeName}{namesNotYetTaken.First()}"
            : $"{podTypeName}{newServiceInstance!.Type}-{Random.Shared.Next(0, int.MaxValue)}";
        return newContainerName;
    }

    public async Task SetResources(IPodInstance podInstance, long nanoCpus, long memory)
    {
        await _environmentClient.SetResources(podInstance, nanoCpus);

        //todo aware of possible race condition but highly unlikely considering the use case (call to this is single threaded)
        _podReadService.GetAllServiceTypes()
            .ForEach(serviceTypes => serviceTypes.ContainerConfig.Config.HostConfig.Memory = memory);
    }

    public Task RemoveVolume(string volumeToFailName)
    {
        return _environmentClient.RemoveVolume(volumeToFailName);
    }

    public Task<ImmutableList<string>?> GetRunningServices()
    {
        return _environmentClient.GetRunningServices();
    }
}