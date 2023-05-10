using System.Collections.Immutable;

namespace Entities.DomainInterfaces.Live;

public interface IPodInstance : IEquatable<IPodInstance>
{
    string Id { get; set; }

    string? Type { get; init; }

    ImmutableList<IServiceInstance> ServiceInstances { get; set; }
}