using Docker.DotNet.Models;
using Ingress.Interfaces;

namespace Entities.BusinessObjects;

public class ContainerConfig : IContainerConfig
{
    public ContainerInspectResponse Config { get; set; }
}