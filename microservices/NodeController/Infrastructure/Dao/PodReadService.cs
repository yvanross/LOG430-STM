using System.Collections.Immutable;
using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Dao;
using Entities.BusinessObjects.Planned;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;
using Infrastructure.Cache;

namespace Infrastructure.Dao;

public class PodReadService : IPodReadService
{
    private readonly IHostInfo _hostInfo;

    public PodReadService(IHostInfo hostInfo)
    {
        _hostInfo = hostInfo;
    }

    public IPodInstance? GetPodOfService(IServiceInstance serviceInstance)
    {
        return RouteCache.GetPodInstances()
            .SingleOrDefault(pod => pod.ServiceInstances.Any(s => s.Equals(serviceInstance)));
    }

    public IPodInstance? GetPodById(string id)
    {
        return RouteCache.GetPodInstances().SingleOrDefault(pod => pod.Id.Equals(id));
    }

    public ImmutableList<IPodInstance> GetPodInstances(string? podType)
    {
        if (podType is null) return ImmutableList<IPodInstance>.Empty;

        return RouteCache.GetPodInstances().Where(pod => pod.Type.Equals(podType)).ToImmutableList();
    }

    public ImmutableList<IPodInstance> GetAllPods()
    {
        return RouteCache.GetPodInstances();
    }

    public ImmutableList<IPodType> GetAllPodTypes()
    {
        return RouteCache.GetPodTypes().Select(podType => podType.Value).ToImmutableList();
    }

    public IPodType? GetPodType(string? podType)
    {
        if(podType is null) return null;

        return RouteCache.GetPodTypes().Select(kv => kv.Value).SingleOrDefault(pt => pt.Type.Equals(podType));
    }

    public IServiceInstance? GetServiceById(string id)
    {
        return RouteCache.GetPodInstances()
            .SelectMany(pod => pod.ServiceInstances).SingleOrDefault(service => service.Id.Equals(id));
    }

    public ImmutableList<IServiceInstance> GetServiceInstances(string? serviceType)
    {
        if (serviceType is null) return ImmutableList<IServiceInstance>.Empty;

        return RouteCache.GetPodInstances()
            .SelectMany(pod => pod.ServiceInstances).Where(service => service.Type.Equals(serviceType)).ToImmutableList();
    }

    public ImmutableList<IServiceInstance> GetAllServices()
    {
        return RouteCache.GetPodInstances()
            .SelectMany(pod => pod.ServiceInstances).ToImmutableList();
    }

    public ImmutableList<IServiceType> GetAllServiceTypes()
    {
        return RouteCache.GetPodTypes()
            .SelectMany(pod => pod.Value.ServiceTypes).DistinctBy(serviceType => serviceType.Type).ToImmutableList();
    }

    public IServiceType? GetServiceType(string? serviceType)
    {
        if (serviceType is null) return null;

        return RouteCache.GetPodTypes()
            .SelectMany(pod => pod.Value.ServiceTypes).FirstOrDefault(st => st.Type.Equals(serviceType));
    }

    public string GetAddress()
    {
        return _hostInfo.GetAddress();
    }
}