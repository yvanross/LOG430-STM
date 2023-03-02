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
        var serviceRoute = _repositoryRead.ReadRouteByType(serviceType);

        if (serviceRoute is null)
            throw new Exception("service route was not found");

        return serviceRoute.HttpRoute;
    }

    public async Task a()
    {
        
    }
}