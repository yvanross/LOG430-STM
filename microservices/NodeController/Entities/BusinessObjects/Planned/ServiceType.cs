using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;

namespace Entities.BusinessObjects.Planned;

public class ServiceType : IServiceType
{
    public required string Type { get; set; }

    public required IContainerConfig ContainerConfig { get; set; }

    public string ArtifactType { get; set; } = ArtifactTypeEnum.Undefined.ToString();

    public string DnsAccessibilityModifier { get; set; } = AccessibilityModifierEnum.Private.ToString();
    
    public string PodName { get; set; }
}