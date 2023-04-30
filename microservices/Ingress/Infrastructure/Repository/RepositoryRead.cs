using System.Collections.Immutable;
using ApplicationLogic.Interfaces;
using Entities.DomainInterfaces;
using Infrastructure.Cache;

namespace Infrastructure.Repository;

public class RepositoryRead : IRepositoryRead
{
    public INode? ReadNodeById(string nodeId)
    {
        return RouteCache.GetNode(nodeId);
    }

    public ImmutableList<INode>? GetAllNodes()
    {
        return RouteCache.GetAllNodes();
    }

    public IScheduler GetScheduler()
    {
        return RouteCache.GetScheduler();
    }
}