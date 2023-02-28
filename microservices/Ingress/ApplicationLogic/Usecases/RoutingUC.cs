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

    public string RouteByDestinationType(string serviceType)
    {
        var serviceRoute = _repositoryRead.ReadRouteByType(serviceType);

        if (serviceRoute is null)
            throw new Exception("service route was not found");

        var port = string.IsNullOrEmpty(serviceRoute.PortNumber) ? string.Empty : $":{serviceRoute.PortNumber}";

        return $"https://{serviceRoute.Address}{port}";
    }
}