using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.ResourceManagement;

namespace Entities.BusinessObjects.Live;

public class ServiceInstance : IServiceInstance
{
    public required string Id { get; init; }

    public required ContainerInfo? ContainerInfo { get; set; }

    public required string Address { get; init; }

    public required string Type { get; init; }

    public required string PodId { get; set; }

    public List<string> VolumeIds { get; set; } = new ();

    public required IServiceState ServiceStatus { get; set; }

    public string HttpRoute => $"http://{Address}:{ContainerInfo?.PortsInfo.RoutingPortNumber}";

    public bool Equals(IServiceInstance? other)
    {
        return Id.Equals(other?.Id);
    }
}