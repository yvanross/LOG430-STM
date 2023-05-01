namespace Entities.DomainInterfaces;

public interface INode
{
    public string Name { get; set; }

    public INodeState ServiceStatus { get; set; }

    public DateTime LastSuccessfulPing { get; set; }
}