using System.Collections.Immutable;
using Entities.BusinessObjects;
using Entities.DomainInterfaces;

namespace ApplicationLogic.Interfaces;

public interface IRepositoryRead
{
    INode? ReadNodeById(string nodeId);

    ImmutableList<INode>? GetAllNodes();

    IScheduler GetScheduler();
}