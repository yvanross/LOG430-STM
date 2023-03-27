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

    public DateTime LastHeartbeat { get; set; } = DateTime.UtcNow;

    public bool IsHttp { get; init; } = true;

    public IServiceState? ServiceStatus { get; set; }

    public string HttpRoute => $"http://{Address}" + (string.IsNullOrEmpty(ContainerInfo?.Port) ? string.Empty : $":{ContainerInfo.Port}");

    public string HttpsRoute => $"https://{Address}" + (string.IsNullOrEmpty(ContainerInfo?.Port) ? string.Empty : $":{ContainerInfo.Port}");
    
    public bool Equals(IServiceInstance? other)
    {
        return Id.Equals(other?.Id);
    }
}