using Entities.DomainInterfaces.Live;

namespace Entities.DomainInterfaces.Planned;

public interface IServiceType
{
    string Type { get; set; }

    IContainerConfig ContainerConfig { get; set; }

    string ArtifactType { get; set; }

    bool IsPodSidecar { get; set; }
}