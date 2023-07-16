using Entities.BusinessObjects.Live;
using IO.Swagger.Models;

namespace Entities.DomainInterfaces.Live;

public interface IContainerConfig
{
    public ContainerInspectResponse Config { get; set; }

    public PortsInfo PortsInfo { get; set; }

}