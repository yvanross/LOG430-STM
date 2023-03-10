using ApplicationLogic.Interfaces;
using Entities.DomainInterfaces;

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

        var services = _readModel.ReadServiceByType(service.ServiceType);

        if(services.IsEmpty is false)
        _environmentClient.IncreaseByOneNumberOfInstances(services.firs)
    }
}