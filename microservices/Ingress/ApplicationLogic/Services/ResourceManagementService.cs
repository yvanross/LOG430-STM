using System.Collections.Immutable;
using ApplicationLogic.Interfaces;
using Entities.DomainInterfaces;
using System.Linq;
using Ambassador;
using ApplicationLogic.Extensions;
using Entities;
using Entities.BusinessObjects;
using System.ComponentModel;
using Entities.BusinessObjects.States;

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

        _readModel.GetScheduler()?.TryAddTask(MatchInstanceDemandOnServices);
    }


    private async Task MatchInstanceDemandOnServices()
    {
        var serviceTypes = _readModel.GetAllServiceTypes();

        if (serviceTypes is not null)
        {
            foreach (var serviceType in serviceTypes)
            {
                while (_readModel.ReadServiceByType(serviceType.Type)?.Count < serviceType.MinimumNumberOfInstances)
                {
                    await IncreaseNumberOfInstances(serviceType.Type).ConfigureAwait(false);
                }
            }
        }
    }

    public async Task RemoveServiceInstance(IServiceInstance serviceInstance)
    {
        _writeModel.RemoveService(serviceInstance);

        if (serviceInstance.ContainerInfo != null)
            await _environmentClient.RemoveContainerInstance(serviceInstance.ContainerInfo.Id);
    }

    public async Task ReplaceServiceInstance(IServiceInstance serviceInstance)
    {
        _writeModel.RemoveService(serviceInstance);

        if (serviceInstance.ContainerInfo != null)
            await _environmentClient.RemoveContainerInstance(serviceInstance.ContainerInfo.Id);

        serviceInstance.ServiceStatus = new LaunchedState(serviceInstance);

        _writeModel.WriteService(serviceInstance);

        var serviceType = _readModel.GetServiceType(serviceInstance.Type);

        await _environmentClient.IncreaseByOneNumberOfInstances(serviceType.ContainerConfig, $"{serviceType.Type}-{Random.Shared.Next(0, int.MaxValue)}", serviceInstance.Id);
    }

    public async Task IncreaseNumberOfInstances(string type)
    {
        var serviceType = _readModel.GetServiceType(type);

        if (serviceType is not null)
        {
            var id = Guid.NewGuid();

            if(_readModel.ReadServiceById(id) is {} service) _writeModel.RemoveService(service);

            var newServiceInstance = new ServiceInstance()
            {
                Address = _readModel.GetAddress(),
                Id = id,
                ContainerInfo = default,
                Type = serviceType.Type,
            };

            newServiceInstance.ServiceStatus = new LaunchedState(newServiceInstance);

            _writeModel.WriteService(newServiceInstance);

            await _environmentClient.IncreaseByOneNumberOfInstances(serviceType.ContainerConfig, $"{serviceType?.Type}-{Random.Shared.Next(0,int.MaxValue)}", id);
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
}