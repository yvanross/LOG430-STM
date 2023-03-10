using ApplicationLogic.Interfaces;
using Entities.DomainInterfaces;
using Ingress.Cache;
using Ingress.Interfaces;

namespace Ingress.Repository;

public class RepositoryWrite : IRepositoryWrite
{
    public void WriteService(IServiceInstance serviceInstance)
    {
        RouteCache.AddOrUpdateService(serviceInstance);
    }

    public void RemoveService(IServiceInstance serviceInstance)
    {
        RouteCache.RemoveService(serviceInstance);
    }

    public void UpdateServiceType(IServiceInstance serviceInstance, IServiceType containerConfigName)
    {
        RouteCache.UpdateContainerModel(serviceInstance, containerConfigName);
    }
}