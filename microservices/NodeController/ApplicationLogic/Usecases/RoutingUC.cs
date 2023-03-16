using Ambassador;
using Ambassador.BusinessObjects;
using ApplicationLogic.Interfaces;
using ApplicationLogic.Services;
using Entities;
using Entities.DomainInterfaces;

namespace ApplicationLogic.Usecases;

public class RoutingUC
{
    private IRepositoryRead _repositoryRead;

    private ResourceManagementService _resourceManagementService;

    private const string TomtomUrl = "https://api.tomtom.com";

    public RoutingUC(IRepositoryRead repositoryRead, IRepositoryWrite repositoryWrite, IEnvironmentClient environment)
    {
        _repositoryRead = repositoryRead;
        _resourceManagementService = new ResourceManagementService(environment, repositoryRead, repositoryWrite);
    }

    public IEnumerable<RoutingData> RouteByDestinationType(string type, LoadBalancingMode mode)
    {
        if (type.Equals(ServiceTypes.Tomtom.ToString())){ yield return new RoutingData() { Address = TomtomUrl }; yield break; }

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