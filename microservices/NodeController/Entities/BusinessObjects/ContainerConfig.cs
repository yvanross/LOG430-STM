using Docker.DotNet.Models;
using Entities.DomainInterfaces;

namespace Entities.BusinessObjects;

public class ContainerConfig : IContainerConfig
{
    public ContainerInspectResponse Config { get; set; }
}