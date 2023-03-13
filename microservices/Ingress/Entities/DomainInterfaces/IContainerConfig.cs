using Docker.DotNet.Models;

namespace Ingress.Interfaces;

public interface IContainerConfig
{
    public ContainerInspectResponse Config { get; set; }
}