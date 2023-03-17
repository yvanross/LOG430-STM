using Docker.DotNet.Models;
using Entities.DomainInterfaces.Live;

namespace Entities.BusinessObjects.Live;

public class ContainerConfig : IContainerConfig
{
    public ContainerInspectResponse Config { get; set; }
}