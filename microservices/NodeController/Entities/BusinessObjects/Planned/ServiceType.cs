using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;

namespace Entities.BusinessObjects.Planned;

public class ServiceType : IServiceType, IEquatable<ServiceType>
{
    public required string Type { get; set; }

    public required IContainerConfig ContainerConfig { get; set; }

    public string ArtifactType { get; set; } = ArtifactTypeEnum.Undefined.ToString();

    public string DnsAccessibilityModifier { get; set; } = AccessibilityModifierEnum.Private.ToString();

    public bool Equals(ServiceType? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Type == other.Type;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ServiceType)obj);
    }

    public override int GetHashCode()
    {
        return Type.GetHashCode();
    }
}