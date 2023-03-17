using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using ApplicationLogic.Services;
using Entities.BusinessObjects.Planned;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;
using Entities.DomainInterfaces.ResourceManagement;

namespace NodeController.Cache;

internal static class RouteCache
{
    private static readonly ConcurrentDictionary<string, IPodInstance> PodInstances = new ();
    
    private static readonly ConcurrentDictionary<string, IPodType> PodTypes = new ();

    private static readonly Scheduler Scheduler = new ();

    internal static ImmutableList<IPodInstance> GetPodInstances()
    {
        return PodInstances.Select(pod => pod.Value).ToImmutableList();
    }

    internal static Dictionary<string, IPodType> GetPodTypes()
    {
        return PodTypes.ToDictionary(k=>k.Key, v => v.Value);
    }

    internal static void AddOrUpdatePodType(IPodType podType)
    {
        PodTypes.AddOrUpdate(podType.Type, podType, (_, _) => podType);
    }

    internal static void AddOrUpdatePodInstance(IPodInstance podInstance)
    {
        PodInstances.AddOrUpdate(podInstance.Id, podInstance, (_, _) => podInstance);
    }

    internal static void RemovePodInstance(IPodInstance podInstance)
    {
        PodInstances.TryRemove(podInstance.Id, out _);
    }

    internal static IScheduler GetScheduler()
    {
        return Scheduler;
    }
}   