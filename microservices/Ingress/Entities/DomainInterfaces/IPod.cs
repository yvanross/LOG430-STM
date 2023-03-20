using System.Collections.Immutable;

namespace Entities.DomainInterfaces;

public interface IPod
{
    string Id { get; set; }

    string Type { get; init; }
    
    string Category { get; init; }

    ImmutableList<IService> ServiceInstances { get; set; }
}