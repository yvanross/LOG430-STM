using Docker.DotNet.Models;

namespace Entities.DomainInterfaces;

public interface IContainerConfig
{
    public ContainerInspectResponse Config { get; set; }
}