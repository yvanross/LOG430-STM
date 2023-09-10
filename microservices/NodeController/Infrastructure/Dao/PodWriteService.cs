using Entities.Dao;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;
using Infrastructure.Cache;

namespace Infrastructure.Dao;

public class PodWriteService : IPodWriteService
{
    public void AddOrUpdatePod(IPodInstance podInstance)
    {
        RouteCache.AddOrUpdatePodInstance(podInstance);
    }

    public void TryRemovePod(IPodInstance podInstance)
    {
        RouteCache.RemovePodInstance(podInstance);
    }

    public void AddOrUpdateServiceType(IServiceType serviceType)
    {
        RouteCache.AddOrUpdateServiceType(serviceType);
    }

    public void AddOrUpdatePodType(IPodType podType)
    {
        RouteCache.AddOrUpdatePodType(podType);
    }

    public void RemovePodType(IPodType podType)
    {
        RouteCache.RemovePodType(podType);
    }

    public void AddTunnel(int port, IServiceType type)
    {
        RouteCache.AddTunnel(port, type);
    }
}