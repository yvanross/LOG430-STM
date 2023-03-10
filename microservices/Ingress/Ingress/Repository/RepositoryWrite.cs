using ApplicationLogic.Interfaces;
using Entities.DomainInterfaces;
using Ingress.Cache;
using Ingress.Interfaces;

namespace Ingress.Repository;

public class RepositoryWrite : IRepositoryWrite
{
    public void WriteService(IService service)
    {
        RouteCache.AddOrUpdateService(service);
    }

    public void RemoveService(IService service)
    {
        RouteCache.RemoveService(service);
    }

    public void UpdateContainerModel(IService service, IContainerConfigName containerConfigName)
    {
        RouteCache.UpdateContainerModel(service, containerConfigName);
    }
}