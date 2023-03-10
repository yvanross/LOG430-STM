using Entities.DomainInterfaces;

namespace Entities.BusinessObjects;

public class ServiceInstance : IServiceInstance
{
    public required Guid Id { get; init; }
    
    public required ContainerInfo ContainerInfo { get; init; }

    public required string Address { get; init; }
    
    public required string Type { get; init; }

    public DateTime LastHeartbeat { get; set; } = DateTime.UtcNow;

    public bool IsHttp { get; init; } = true;

    public string HttpRoute => $"http://{Address}" + (string.IsNullOrEmpty(ContainerInfo.Port) ? string.Empty : $":{ContainerInfo.Port}");

    public string HttpsRoute => $"https://{Address}" + (string.IsNullOrEmpty(ContainerInfo.Port) ? string.Empty : $":{ContainerInfo.Port}");

}