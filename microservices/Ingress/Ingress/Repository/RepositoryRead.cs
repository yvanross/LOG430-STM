using ApplicationLogic.Interfaces;
using Entities.DomainInterfaces;
using Ingress.Cache;

namespace Ingress.Repository;

public class RepositoryRead : IRepositoryRead
{
    public IRoute? ReadRouteById(string id)
    {
        return RouteCache.RegisteredRoutes.SingleOrDefault(route => route.Id.Equals(id));
    }

    public IRoute? ReadRouteByAddressAndPort(string address, string port)
    {
        return RouteCache.RegisteredRoutes.SingleOrDefault(route => route.Address.Equals(address) && route.PortNumber.Equals(port));
    }

    public IRoute? ReadRouteByType(string serviceType)
    {
        return RouteCache.RegisteredRoutes.SingleOrDefault(route => route.ServiceType.Equals(serviceType));
    }
}