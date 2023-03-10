using ApplicationLogic.Interfaces;
using Entities.DomainInterfaces;
using System.Linq;

namespace ApplicationLogic.Services;

public class LoadBalancingService
{
    private readonly IEnvironmentClient _environmentClient;

    private readonly IRepositoryRead _readModel;

    public LoadBalancingService(IEnvironmentClient environmentClient)
    {
        _environmentClient = environmentClient;
    }

    public void IncreaseByOneNumberOfInstances(IService service)
    {
        var containerId = string.Empty;

        var containerConfig = _readModel.GetContainerModel(service.ServiceType);

        if (containerConfig is not null)
        {
            _environmentClient.IncreaseByOneNumberOfInstances(containerConfig, $"{containerId}_{Guid.NewGuid()}");
        }
    }
}