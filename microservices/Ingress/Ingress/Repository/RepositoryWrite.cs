using ApplicationLogic.Interfaces;
using Entities.DomainInterfaces;
using Ingress.Cache;

namespace Ingress.Repository;

public class RepositoryWrite : IRepositoryWrite
{
    public void Write(IService service)
    {
        RouteCache.AddOrUpdateService(service);
    }
}