using Ambassador;
using Ambassador.BusinessObjects;
using ApplicationLogic.Interfaces;
using ApplicationLogic.Services;
using Docker.DotNet;
using Entities.DomainInterfaces;

namespace ApplicationLogic.Usecases;

public class RoutingUC
{
    private IRepositoryRead _repositoryRead;

    private ResourceManagementService _resourceManagementService;

    public RoutingUC(IRepositoryRead repositoryRead, IEnvironmentClient environment)
    {
        _repositoryRead = repositoryRead;
        _resourceManagementService = new ResourceManagementService(environment);
    }

    public IEnumerable<RoutingData> RouteByDestinationType(string type, LoadBalancingMode mode)
    {
        var serviceRoute = _repositoryRead.ReadServiceByType(type);

        if (serviceRoute is null)
            throw new Exception("service was not found");

        var targets = _resourceManagementService.LoadBalancing(serviceRoute, mode);

        foreach (var target in targets)
        {
            yield return new RoutingData()
            {
                Address = target.IsHttp ? target.HttpRoute : target.HttpsRoute
            };
        }
    }
}