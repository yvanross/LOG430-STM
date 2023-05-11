using System.Collections.Immutable;

namespace Entities.DomainInterfaces.Planned;

public interface IPodType
{
    string? Type { get; set; }

    int NumberOfInstances { get; set; }

    ImmutableList<IServiceType> ServiceTypes { get; set; }
}
