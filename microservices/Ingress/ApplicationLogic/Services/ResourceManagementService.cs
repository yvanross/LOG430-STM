using System.Collections.Immutable;
using ApplicationLogic.Interfaces;
using Entities.DomainInterfaces;
using System.Linq;
using Ambassador;
using ApplicationLogic.Extensions;

namespace ApplicationLogic.Services;

public class ResourceManagementService
{
    private readonly IEnvironmentClient _environmentClient;

    private readonly IRepositoryRead _readModel;

    private readonly Random _random = new Random();

    public ResourceManagementService(IEnvironmentClient environmentClient)
    {
        _environmentClient = environmentClient;
    }

    public void IncreaseNumberOfInstances(IServiceInstance serviceInstance)
    {
        var containerId = string.Empty;

        var serviceType = _readModel.GetServiceType(serviceInstance.Type);

        if (serviceType is not null)
        {
            _environmentClient.IncreaseByOneNumberOfInstances(serviceType.ContainerConfig, $"{containerId}_{Guid.NewGuid()}");
        }
    }

    public ImmutableList<IServiceInstance> LoadBalancing(ImmutableList<IServiceInstance> services, LoadBalancingMode mode)
    {
        return mode switch
        {
            LoadBalancingMode.RoundRobin => ImmutableList.Create(services[_random.Next(0, services.Count - 1)]),
            LoadBalancingMode.Broadcast => services,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }
}