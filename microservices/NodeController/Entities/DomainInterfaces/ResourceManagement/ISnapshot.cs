using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;

namespace Entities.DomainInterfaces.ResourceManagement;

public interface ISnapshot
{
    DateTime Timestamp { get; }

    List<IServiceInstance> RunningInstances { get; }
    
    List<IServiceType> ServiceTypes { get; }

    IEnumerable<ISaga> Saga { get; }
}