using System.Collections.Immutable;

namespace Entities.DomainInterfaces.Planned;

public interface IPodType
{
    string Type { get; set; }

    int MinimumNumberOfInstances { get; set; }

    IServiceType? Gateway { get; set; }

    ImmutableList<IServiceType> ServiceTypes { get; set; }
}
