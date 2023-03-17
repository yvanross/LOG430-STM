using Entities.BusinessObjects.Live;
using Entities.DomainInterfaces.ResourceManagement;

namespace Entities.DomainInterfaces.Live;

public interface IServiceInstance : IEquatable<IServiceInstance>
{
    string Id { get; init; }

    ContainerInfo? ContainerInfo { get; set; }

    string Address { get; init; }

    string Type { get; init; }

    DateTime LastHeartbeat { get; set; }

    bool IsHttp { get; init; }

    IServiceState? ServiceStatus { get; set; }

    string HttpRoute { get; }

    string HttpsRoute { get; }

    string PodId { get; set; }
}