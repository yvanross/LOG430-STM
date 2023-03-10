using Entities.BusinessObjects;

namespace Entities.DomainInterfaces;

public interface IService
{
    Guid Id { get; init; }

    ContainerInfo ContainerInfo { get; init; }

    string Address { get; init; }

    string ServiceType { get; init; }

    double Latency { get; set; }
    
    DateTime LastHeartbeat { get; set; }

    bool IsHttp { get; init; }

    string HttpRoute { get; }
    
    string HttpsRoute { get; }
}