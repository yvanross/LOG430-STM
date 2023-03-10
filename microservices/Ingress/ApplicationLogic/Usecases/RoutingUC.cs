using ApplicationLogic.Interfaces;
using Docker.DotNet;
using Entities.DomainInterfaces;

namespace ApplicationLogic.Usecases;

public class RoutingUC
{
    private IRepositoryRead _repositoryRead;

    public RoutingUC(IRepositoryRead repositoryRead)
    {
        _repositoryRead = repositoryRead;
    }

    public string RouteByDestinationType(string serviceType)
    {
        var serviceRoute = _repositoryRead.ReadServiceByType(serviceType);

        if (serviceRoute is null)
            throw new Exception("service service was not found");

        return serviceRoute.IsHttp ? serviceRoute.HttpRoute : serviceRoute.HttpsRoute;
    }
}