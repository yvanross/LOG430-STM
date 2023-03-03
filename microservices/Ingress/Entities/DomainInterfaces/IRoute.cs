namespace Entities.DomainInterfaces;

public interface IRoute
{
    string Id { get; init; }

    string Address { get; init; }

    string Port { get; init; }

    string ServiceType { get; init; }

    double Latency { get; set; }

    bool IsHttp { get; init; }

    string HttpRoute { get; }
    
    string HttpsRoute { get; }
}