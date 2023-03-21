using System.Collections.Immutable;
using ApplicationLogic.Interfaces;
using Ambassador;
using Entities.BusinessObjects.States;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.ResourceManagement;
using Entities.BusinessObjects.Live;

namespace ApplicationLogic.Services;

public class ResourceManagementService
{
    private readonly IEnvironmentClient _environmentClient;

    private readonly IRepositoryRead _readModel;
    
    private readonly IRepositoryWrite _writeModel;

    public ResourceManagementService(IEnvironmentClient environmentClient, IRepositoryRead readModel, IRepositoryWrite writeModel)
    {
        _environmentClient = environmentClient;
        _readModel = readModel;
        _writeModel = writeModel;

        _readModel.GetScheduler()?.TryAddTask(nameof(MatchInstanceDemandOnPods), MatchInstanceDemandOnPods);
    }


    private async Task MatchInstanceDemandOnPods()
    {
        var podTypes = _readModel.GetAllPodTypes();

        if (podTypes is not null)
        {
            foreach (var podType in podTypes)
            {
                while (_readModel.GetPodInstances(podType.Type)?.Count < podType.MinimumNumberOfInstances)
                {
                    await IncreaseNumberOfPodInstances(podType.Type).ConfigureAwait(false);
                }
            }
        }
    }

    public async Task RemovePodInstance(IPodInstance podInstance)
    {
        _writeModel.TryRemovePod(podInstance);

        foreach (var serviceInstance in podInstance.ServiceInstances)
        {
            if (serviceInstance.ContainerInfo is not null)
                await _environmentClient.RemoveContainerInstance(serviceInstance.ContainerInfo.Id);
        }
        
    }

    public async Task ReplacePodInstance(IPodInstance podInstance)
    {
        await RemovePodInstance(podInstance);

        podInstance.Id = Guid.NewGuid().ToString();

        try
        {
            foreach (var serviceInstance in podInstance.ServiceInstances)
            {
                serviceInstance.ServiceStatus = new LaunchedState(serviceInstance);

                var serviceType = _readModel.GetServiceType(serviceInstance.Type);

                serviceInstance.PodId = podInstance.Id;

                if (serviceType is not null)
                    await _environmentClient.IncreaseByOneNumberOfInstances(serviceType.ContainerConfig, $"{serviceType.Type}-{Random.Shared.Next(0, int.MaxValue)}", serviceInstance.Id, serviceInstance.PodId);
            }
        }
        finally
        {
            _writeModel.AddOrUpdatePod(podInstance);
        }
    }

    public async Task IncreaseNumberOfPodInstances(string type)
    {
        var podType = _readModel.GetPodType(type);

        if (podType is not null)
        {
            var newPodInstance = CreateNewPodInstance();

            try
            {
                foreach (var newServiceInstance in newPodInstance.ServiceInstances)
                {
                    newServiceInstance.ServiceStatus = new LaunchedState(newServiceInstance);

                    var serviceType = _readModel.GetServiceType(newServiceInstance.Type);

                    if (serviceType is not null)
                    {
                        var newContainerName = $"{newServiceInstance!.Type}-{Random.Shared.Next(0, int.MaxValue)}";

                        var creationInfo = await _environmentClient.IncreaseByOneNumberOfInstances(serviceType.ContainerConfig, newContainerName, newServiceInstance.Id, newServiceInstance.PodId);

                        var containerInfo = await _environmentClient.GetContainerInfo(creationInfo!.ID);

                        newServiceInstance.ContainerInfo = containerInfo.CuratedInfo;
                    }
                }

                _writeModel.AddOrUpdatePod(newPodInstance);
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
                    Address = _readModel.GetAddress(),
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

    public ImmutableList<IServiceInstance> LoadBalancing(ImmutableList<IServiceInstance> services, LoadBalancingMode mode)
    {
        services = services.Where(t => t.ServiceStatus is ReadyState).ToImmutableList();

        return mode switch
        {
            LoadBalancingMode.RoundRobin => ImmutableList.Create(services[Random.Shared.Next(0, services.Count - 1)]),
            LoadBalancingMode.Broadcast => services,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }

    public async Task SetResources(IPodInstance podInstance, long nanoCpus, long memory)
    {
        await _environmentClient.SetResources(podInstance, nanoCpus, memory);
    }
}