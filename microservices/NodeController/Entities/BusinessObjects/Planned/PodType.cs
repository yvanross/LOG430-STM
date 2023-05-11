using System.Collections.Immutable;
using Entities.DomainInterfaces.Planned;

namespace Entities.BusinessObjects.Planned;

public class PodType : IPodType
{
    public required string? Type { get; set; }

    public required int NumberOfInstances { get; set; }

    public required ImmutableList<IServiceType> ServiceTypes { get; set; } = ImmutableList<IServiceType>.Empty;

}