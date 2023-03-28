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

    private readonly IPodReadModel _readModelModel;
    
    private readonly IPodWriteModel _writeModelModel;

    public ResourceManagementService(IEnvironmentClient environmentClient, IPodReadModel readModelModel, IPodWriteModel writeModelModel)
    {
        _environmentClient = environmentClient;
        _readModelModel = readModelModel;
        _writeModelModel = writeModelModel;
    }

    public async Task RemovePodInstance(IPodInstance podInstance)
    {
        _writeModelModel.TryRemovePod(podInstance);

        foreach (var serviceInstance in podInstance.ServiceInstances)
        {
            if (serviceInstance.ContainerInfo is not null)
                await _environmentClient.RemoveContainerInstance(serviceInstance.ContainerInfo.Id);
        }
    }

    public async Task ReplacePodInstance(IPodInstance podInstance)
    {
        await RemovePodInstance(podInstance);

        try
        {
            foreach (var serviceInstance in podInstance.ServiceInstances)
            {
                serviceInstance.ServiceStatus = new LaunchedState();

                var serviceType = _readModelModel.GetServiceType(serviceInstance.Type);

                if (serviceType is not null)
                {
                    var creationInfo = await _environmentClient.IncreaseByOneNumberOfInstances(serviceType.ContainerConfig, $"{serviceType.Type}-{Random.Shared.Next(0, int.MaxValue)}", serviceInstance.Id, serviceInstance.PodId);

                    var containerInfo = await _environmentClient.GetContainerInfo(creationInfo!.ID);

                    serviceInstance.ContainerInfo = containerInfo.CuratedInfo;
                }
            }
        }
        finally
        {
            _writeModelModel.AddOrUpdatePod(podInstance);
        }
    }

    public async Task IncreaseNumberOfPodInstances(string type)
    {
        var podType = _readModelModel.GetPodType(type);

        if (podType is not null)
        {
            var newPodInstance = CreateNewPodInstance();

            try
            {
                foreach (var newServiceInstance in newPodInstance.ServiceInstances)
                {
                    newServiceInstance.ServiceStatus = new LaunchedState();

                    var serviceType = _readModelModel.GetServiceType(newServiceInstance.Type);

                    if (serviceType is not null)
                    {
                        var newContainerName = $"{newServiceInstance!.Type}-{Random.Shared.Next(0, int.MaxValue)}";

                        var creationInfo = await _environmentClient.IncreaseByOneNumberOfInstances(serviceType.ContainerConfig, newContainerName, newServiceInstance.Id, newServiceInstance.PodId);

                        var containerInfo = await _environmentClient.GetContainerInfo(creationInfo!.ID);

                        newServiceInstance.ContainerInfo = containerInfo.CuratedInfo;
                    }
                }

                _writeModelModel.AddOrUpdatePod(newPodInstance);
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
                    Address = _readModelModel.GetAddress(),
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
        await _environmentClient.SetResources(podInstance, nanoCpus, memory);
    }
}