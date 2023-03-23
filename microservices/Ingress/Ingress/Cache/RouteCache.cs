using System.Collections.Concurrent;
using System.Collections.Immutable;
using Entities.BusinessObjects;
using Entities.DomainInterfaces;

namespace Ingress.Cache;

internal static class RouteCache
{
    //The portainer Address is the key
    private static readonly ConcurrentDictionary<string, INode> NodeByName = new ();
    
    private static readonly IScheduler Scheduler = new Scheduler();

    internal static INode? GetNode(string id)
    {
        NodeByName.TryGetValue(id, out var value);

        return value;
    }

    internal static ImmutableList<INode> GetAllNodes()
    {
        return NodeByName.Values.ToImmutableList();
    }

    internal static void AddOrUpdateNode(INode node)
    {
        NodeByName.AddOrUpdate(node.Name, (_) => node, (_, _)=> node);
    }

    internal static void RemoveNode(INode node)
    {
        NodeByName.TryRemove(node.Name, out _);
    }

    internal static IScheduler GetScheduler() => Scheduler;
}   