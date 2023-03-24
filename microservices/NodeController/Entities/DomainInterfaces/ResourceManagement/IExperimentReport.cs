using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;

namespace Entities.DomainInterfaces.ResourceManagement;

public interface IExperimentReport
{
    DateTime Timestamp { get; }

    List<IServiceInstance> RunningInstances { get; }
    
    List<IServiceType> ServiceTypes { get; }

    IExperimentResult? ExperimentResult { get; }
}