using Entities.DomainInterfaces;

namespace Entities.BusinessObjects;

public class Route : IRoute
{
    public string Id { get; init; } = Guid.NewGuid().ToString();

    public required string Address { get; init; }
    
    public required string Port { get; init; }

    public required string ServiceType { get; init; }

    public double Latency { get; set; }

    public bool IsHttp { get; init; } = true;

    public string HttpRoute => $"http://{Address}" + (string.IsNullOrEmpty(Port) ? string.Empty : $":{Port}");

    public string HttpsRoute => $"https://{Address}" + (string.IsNullOrEmpty(Port) ? string.Empty : $":{Port}");
}