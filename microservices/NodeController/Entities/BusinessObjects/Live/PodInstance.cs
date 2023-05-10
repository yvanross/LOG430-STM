using System.Collections.Immutable;
using Entities.DomainInterfaces.Live;

namespace Entities.BusinessObjects.Live;

public class PodInstance : IPodInstance
{
    public required string Id { get; set; } 
    public required string? Type { get; init; }
    public required ImmutableList<IServiceInstance> ServiceInstances { get; set; }

    public bool Equals(IPodInstance? other)
    {
        if (other is null) return false;

        return Id.Equals(other.Id) && Type == other.Type && ServiceInstances.Equals(other.ServiceInstances);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((PodInstance)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Type, ServiceInstances);
    }
}