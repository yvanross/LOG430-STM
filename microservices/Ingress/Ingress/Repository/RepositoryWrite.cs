using ApplicationLogic.Interfaces;
using Entities.DomainInterfaces;
using Ingress.Cache;

namespace Ingress.Repository;

public class RepositoryWrite : IRepositoryWrite
{
    public void AddOrUpdateNode(INode node)
    {
        RouteCache.AddOrUpdateNode(node);
    }

    public void RemoveNode(INode node)
    {
        RouteCache.RemoveNode(node);
    }
}