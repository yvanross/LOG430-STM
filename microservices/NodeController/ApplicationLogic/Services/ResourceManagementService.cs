using System.Collections.Immutable;
using ApplicationLogic.Interfaces;
using Entities.BusinessObjects.States;
using Entities.DomainInterfaces.Live;
using Entities.BusinessObjects.Live;
using ApplicationLogic.Interfaces.Dao;

namespace ApplicationLogic.Services;

public class ResourceManagementService
{
    private readonly IEnvironmentClient _environmentClient;

    private readonly IPodReadService _readServiceService;
    
    private readonly IPodWriteService _writeServiceService;

    public ResourceManagementService(IEnvironmentClient environmentClient, IPodReadService readServiceService, IPodWriteService writeServiceService)
    {
        _environmentClient = environmentClient;
        _readServiceService = readServiceService;
        _writeServiceService = writeServiceService;
    }

    public async Task RemovePodInstance(IPodInstance podInstance)
    {
        foreach (var serviceInstance in podInstance.ServiceInstances)
        {
            await _environmentClient.RemoveContainerInstance(serviceInstance.ContainerInfo.Id);
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

                var serviceType = _readServiceService.GetServiceType(serviceInstance.Type);

                if (serviceType is not null)
                {
                    var podTypeName = string.Empty;

                    if (serviceType.Type.Equals(podInstance.Type) is false)
                        podTypeName = podInstance.Type + "-";

                    var creationId = await _environmentClient.IncreaseByOneNumberOfInstances(serviceType.ContainerConfig, $"{podTypeName}{serviceType.Type}-{Random.Shared.Next(0, int.MaxValue)}", serviceInstance.Id, serviceInstance.PodId);

                    var containerInfo = await _environmentClient.GetContainerInfo(creationId);

                    serviceInstance.ContainerInfo = containerInfo.CuratedInfo;
                }
            }

            _writeServiceService.AddOrUpdatePod(podInstance);

        }
        catch
        {
            await RemovePodInstance(podInstance);
        }
    }

    public async Task IncreaseNumberOfPodInstances(string? type)
    {
        var podType = _readServiceService.GetPodType(type);

        if (podType is not null)
        {
            var newPodInstance = CreateNewPodInstance();

            try
            {
                foreach (var newServiceInstance in newPodInstance.ServiceInstances)
                {
                    newServiceInstance.ServiceStatus = new LaunchedState();

                    var serviceType = _readServiceService.GetServiceType(newServiceInstance.Type);

                    if (serviceType is not null)
                    {
                        var podTypeName = string.Empty;

                        if (serviceType.Type.Equals(podType.Type) is false)
                            podTypeName = podType.Type + "-";

                        var newContainerName = $"{podTypeName}{newServiceInstance!.Type}-{Random.Shared.Next(0, int.MaxValue)}";

                        var creationInfo = await _environmentClient.IncreaseByOneNumberOfInstances(serviceType.ContainerConfig, newContainerName, newServiceInstance.Id, newServiceInstance.PodId);

                        var containerInfo = await _environmentClient.GetContainerInfo(creationInfo);

                        newServiceInstance.ContainerInfo = containerInfo.CuratedInfo;
                    }
                }

                _writeServiceService.AddOrUpdatePod(newPodInstance);
            }
            catch
            {
                await RemovePodInstance(newPodInstance);
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
                    Address = _readServiceService.GetAddress(),
                    Id = newServiceId,
                    ContainerInfo = default,
                    Type = serviceType.Type,
                    PodId = newPodId
                });
            }

            var newPodInstance = new PodInstance()
            {
                Id = newPodId,
                ServiceInstances = serviceInstances.ToImmutableList(),
                Type = podType.Type,
            };

            return newPodInstance;
        }
    }

    public async Task SetResources(IPodInstance podInstance, long nanoCpus, long memory)
    {
        await _environmentClient.SetResources(podInstance, nanoCpus);

        //todo aware of possible race condition but highly unlikely considering the use case (call to this is single threaded)
        _readServiceService.GetAllServiceTypes()
            .ForEach(serviceTypes => serviceTypes.ContainerConfig.Config.HostConfig.Memory = memory);
    }
}