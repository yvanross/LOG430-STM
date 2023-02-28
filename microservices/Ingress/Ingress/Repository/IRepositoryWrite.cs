using ApplicationLogic.Interfaces;
using Entities.DomainInterfaces;
using Ingress.Cache;

namespace Ingress.Repository;

public class RepositoryWrite : IRepositoryWrite
{
    public void Write(IRoute route)
    {
        RouteCache.AddRoute(route);
    }
}