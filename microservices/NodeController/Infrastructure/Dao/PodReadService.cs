using System.Collections.Immutable;
using Entities.Dao;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;
using Entities.Extensions;
using Infrastructure.Cache;

namespace Infrastructure.Dao;

public class PodReadService : IPodReadService
{
    //---------------------------------------------------------------- 
    // PODS
    //----------------------------------------------------------------

    public IPodInstance? GetPodOfService(IServiceInstance serviceInstance)
    {
        return RouteCache.GetPodInstances()
            .SingleOrDefault(pod => pod.ServiceInstances.Any(s => s.Id.EqualsIgnoreCase(serviceInstance.Id)));
    }

    public IPodInstance? GetPodById(string id)
    {
        return RouteCache.GetPodInstance(id);
    }

    public ImmutableList<IPodInstance> GetPodInstances(string? podType)
    {
        return podType is null ? 
            ImmutableList<IPodInstance>.Empty : 
            RouteCache.GetPodInstances().Where(pod => pod.Type.EqualsIgnoreCase(podType)).ToImmutableList();
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
        return podType is null ? null : RouteCache.GetPodType(podType);
    }

    //----------------------------------------------------------------
    // Services
    //----------------------------------------------------------------

    public IServiceInstance? GetServiceById(string id)
    {
        return RouteCache.GetPodInstances()
            .SelectMany(pod => pod.ServiceInstances).SingleOrDefault(service => service.Id.EqualsIgnoreCase(id));
    }

    public ImmutableList<IServiceInstance> GetServiceInstances(string? serviceType)
    {
        if (serviceType is null) return ImmutableList<IServiceInstance>.Empty;

        return RouteCache.GetPodInstances()
            .SelectMany(pod => pod.ServiceInstances).Where(service => service.Type.EqualsIgnoreCase(serviceType)).ToImmutableList();
    }

    public ImmutableList<IServiceInstance> GetAllServices()
    {
        return RouteCache.GetPodInstances()
            .SelectMany(pod => pod.ServiceInstances).ToImmutableList();
    }

    public ImmutableList<IServiceType> GetAllServiceTypes()
    {
        return RouteCache.GetServiceTypes().Select(st=>st.Value).ToImmutableList();
    }

    public IServiceType? GetServiceType(string? serviceType)
    {
        return serviceType is null ? null : RouteCache.GetServiceType(serviceType);
    }

    //----------------------------------------------------------------
    // Tunnels
    //----------------------------------------------------------------

    public ImmutableList<int> GetTakenSocketPorts()
    {
        return RouteCache.GetAllPorts();
    }

    public int? TryGetSocketPortForType(IServiceType type)
    {
        return RouteCache.GetPortForType(type);
    }

    public IServiceType? TryGetServiceTypeFromPort(int port)
    {
        return RouteCache.TryGetServiceTypeFromPort(port);
    }
}