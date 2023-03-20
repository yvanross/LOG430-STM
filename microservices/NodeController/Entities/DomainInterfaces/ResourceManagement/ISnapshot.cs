using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;

namespace Entities.DomainInterfaces.ResourceManagement;

public interface ISnapshot
{
    List<IServiceInstance> RunningInstances { get; }
    
    List<IServiceType> ServiceTypes { get; }
}