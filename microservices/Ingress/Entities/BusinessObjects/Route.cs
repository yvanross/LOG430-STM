using Entities.DomainInterfaces;

namespace Entities.BusinessObjects;

public class Route : IRoute
{
    public string Id { get; init; } = Guid.NewGuid().ToString();

    public required string Address { get; init; }
    
    public required string PortNumber { get; init; }

    public required string ServiceType { get; init; }

    public double Latency { get; set; }
}