namespace Entities.DomainInterfaces;

public interface IRoute
{
    string Id { get; init; }

    string Address { get; init; }

    string PortNumber { get; init; }

    string ServiceType { get; init; }

    double Latency { get; set; }
}