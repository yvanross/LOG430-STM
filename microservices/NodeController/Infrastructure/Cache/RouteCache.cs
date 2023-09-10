using System.Collections.Concurrent;
using System.Collections.Immutable;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;

namespace Infrastructure.Cache;

internal static class RouteCache
{
    private static readonly ConcurrentDictionary<string, IPodInstance> PodInstances = new ();

    private static readonly ConcurrentDictionary<string, IServiceType> ServiceTypes = new();

    private static readonly ConcurrentDictionary<string, IPodType> PodTypes = new ();
    
    private static readonly ConcurrentDictionary<int, IServiceType> Tunnels = new ();

    internal static IPodInstance? GetPodInstance(string podId)
    {
        PodInstances.TryGetValue(podId, out var podInstance);

        return podInstance;
    }

    internal static ImmutableList<IPodInstance> GetPodInstances()
    {
        return PodInstances.Select(pod => pod.Value).ToImmutableList();
    }

    internal static IServiceType? GetServiceType(string serviceType)
    {
        ServiceTypes.TryGetValue(serviceType, out var value);

        return value;
    }

    internal static Dictionary<string, IServiceType> GetServiceTypes()
    {
        return ServiceTypes.ToDictionary(k => k.Key, v => v.Value);
    }

    internal static IPodType? GetPodType(string podType)
    {
        PodTypes.TryGetValue(podType, out var value);

        return value;
    }

    internal static Dictionary<string, IPodType> GetPodTypes()
    {
        return PodTypes.ToDictionary(k=>k.Key, v => v.Value);
    }

    internal static void AddOrUpdatePodInstance(IPodInstance podInstance)
    {
        PodInstances.AddOrUpdate(podInstance.Id, podInstance, (_, _) => podInstance);
    }

    internal static void AddOrUpdateServiceType(IServiceType serviceType)
    {
        ServiceTypes.AddOrUpdate(serviceType.Type, serviceType, (_, _) => serviceType);
    }

    internal static void AddOrUpdatePodType(IPodType podType)
    {
        PodTypes.AddOrUpdate(podType.Type, podType, (_, _) => podType);
    }

    internal static void RemovePodType(IPodType podType)
    {
        PodTypes.TryRemove(podType.Type, out _);
    }

    internal static void RemovePodInstance(IPodInstance podInstance)
    {
        PodInstances.TryRemove(podInstance.Id, out _);
    }

    internal static ImmutableList<int> GetAllPorts()
    {
        return Tunnels.Select(kv => kv.Key).ToImmutableList();
    }

    internal static int? GetPortForType(IServiceType serviceType)
    {
        return Tunnels.SingleOrDefault(tunnel =>
            tunnel.Value.Type.Equals(serviceType.Type, StringComparison.InvariantCultureIgnoreCase)).Key;
    }

    internal static void AddTunnel(int port, IServiceType type)
    {
        Tunnels.AddOrUpdate(port, type, (_, _) => type);
    }

    public static IServiceType? TryGetServiceTypeFromPort(int port)
    {
        Tunnels.TryGetValue(port, out var value);

        return value;
    }
}   