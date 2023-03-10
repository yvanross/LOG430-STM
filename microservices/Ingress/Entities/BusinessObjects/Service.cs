using Entities.DomainInterfaces;

namespace Entities.BusinessObjects;

public class Service : IService
{
    public required Guid Id { get; init; }
    
    public required ContainerInfo ContainerInfo { get; init; }

    public bool AutoScaleInstances { get; }
    
    public int MinimumNumberOfInstances { get; }

    public required string Address { get; init; }
    
    public required string ServiceType { get; init; }

    public double Latency { get; set; }

    public DateTime LastHeartbeat { get; set; } = DateTime.UtcNow;

    public bool IsHttp { get; init; } = true;

    public string HttpRoute => $"http://{Address}" + (string.IsNullOrEmpty(ContainerInfo.Port) ? string.Empty : $":{ContainerInfo.Port}");

    public string HttpsRoute => $"https://{Address}" + (string.IsNullOrEmpty(ContainerInfo.Port) ? string.Empty : $":{ContainerInfo.Port}");

}