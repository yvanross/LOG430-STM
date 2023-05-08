namespace Entities.DomainInterfaces;

public interface INode
{
    string Name { get; set; }

    INodeState ServiceStatus { get; set; }

    DateTime LastSuccessfulPing { get; set; }

    string Version { get; set; }

    bool Secure { get; set; }
    
    bool Dirty { get; set; }
}