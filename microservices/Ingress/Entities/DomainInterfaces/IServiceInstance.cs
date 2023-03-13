using Entities.BusinessObjects;

namespace Entities.DomainInterfaces;

public interface IServiceInstance
{
    Guid Id { get; init; }

    ContainerInfo? ContainerInfo { get; init; }

    string Address { get; init; }

    string Type { get; init; }

    DateTime LastHeartbeat { get; set; }

    bool IsHttp { get; init; }

    IServiceState? ServiceStatus { get; set; }

    string HttpRoute { get; }
    
    string HttpsRoute { get; }
}