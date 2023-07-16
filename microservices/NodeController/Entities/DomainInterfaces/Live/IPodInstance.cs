using Entities.DomainInterfaces.ResourceManagement;
using System.Collections.Immutable;

namespace Entities.DomainInterfaces.Live;

public interface IPodInstance : IEquatable<IPodInstance>
{
    string Id { get; set; }

    string Type { get; init; }

    ImmutableList<IServiceInstance> ServiceInstances { get; set; }

    IServiceState ServiceStatus { get; set; }

    void AddServiceInstance(IServiceInstance serviceInstance);

    void ReplaceServiceInstance(IServiceInstance serviceInstance);
}