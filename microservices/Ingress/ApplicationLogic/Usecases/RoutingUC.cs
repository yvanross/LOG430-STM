using ApplicationLogic.Interfaces;
using Entities.DomainInterfaces;

namespace ApplicationLogic.Usecases;

public class RoutingUC
{
    private IRepositoryRead _repositoryRead;

    public RoutingUC(IRepositoryRead repositoryRead)
    {
        _repositoryRead = repositoryRead;
    }

    public string? RouteByDestinationType(string serviceType)
    {
        var serviceRoute = _repositoryRead.ReadRouteByType(serviceType);

        return serviceRoute?.Address;
    }
}