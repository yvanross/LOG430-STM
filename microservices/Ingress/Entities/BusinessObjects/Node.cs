using Entities.BusinessObjects.States;
using Entities.DomainInterfaces;
using Entities.DomainInterfaces.ResourceManagement;

namespace Entities.BusinessObjects;

public class Node : INode
{
    public required string Name { get; set; }

    public required string Address { get; set; }

    public required string Port { get; set; }

    public INodeState ServiceStatus { get; set; } = new ReadyState();

    public DateTime LastSuccessfulPing { get; set; } = DateTime.UtcNow;
}