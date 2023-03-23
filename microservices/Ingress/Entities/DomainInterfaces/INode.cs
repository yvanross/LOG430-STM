using Entities.DomainInterfaces.ResourceManagement;

namespace Entities.DomainInterfaces;

public interface INode
{
    public string Name { get; set; }

    public string Address { get; set; }

    public string Port { get; set; }

    public INodeState ServiceStatus { get; set; }

    public DateTime LastSuccessfulPing { get; set; }
}