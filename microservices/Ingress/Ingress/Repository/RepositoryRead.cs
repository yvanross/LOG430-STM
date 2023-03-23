using System.Collections.Immutable;
using ApplicationLogic.Interfaces;
using Entities.BusinessObjects;
using Entities.DomainInterfaces;
using Ingress.Cache;

namespace Ingress.Repository;

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