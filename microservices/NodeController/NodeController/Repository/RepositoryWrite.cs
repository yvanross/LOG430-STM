using ApplicationLogic.Interfaces;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;
using NodeController.Cache;

namespace NodeController.Repository;

public class RepositoryWrite : IRepositoryWrite
{
    public void AddOrUpdatePod(IPodInstance podInstance)
    {
        RouteCache.AddOrUpdatePodInstance(podInstance);
    }

    public void TryRemovePod(IPodInstance podInstance)
    {
        RouteCache.RemovePodInstance(podInstance);
    }

    public void AddOrUpdatePodType(IPodType podType)
    {
        RouteCache.AddOrUpdatePodType(podType);
    }
}