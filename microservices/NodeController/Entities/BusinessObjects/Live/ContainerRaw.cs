using Entities.DomainInterfaces.Live;
using IO.Swagger.Models;

namespace Entities.BusinessObjects.Live;

public class ContainerRaw : IContainerConfig
{
    public ContainerInspectResponse Config { get; set; }

    public required PortsInfo PortsInfo { get; set; }
}