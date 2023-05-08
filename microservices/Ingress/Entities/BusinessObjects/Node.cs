using Entities.BusinessObjects.States;
using Entities.DomainInterfaces;

namespace Entities.BusinessObjects;

public class Node : INode
{
    public required string Name { get; set; }

    public INodeState ServiceStatus { get; set; } = new ReadyState();

    public DateTime LastSuccessfulPing { get; set; } = DateTime.UtcNow;
    
    public required string Version { get; set; }

    public bool Secure { get; set; } = false;

    public bool Dirty { get; set; } = true;
}