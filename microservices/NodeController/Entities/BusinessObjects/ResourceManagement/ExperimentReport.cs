using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;
using Entities.DomainInterfaces.ResourceManagement;

namespace Entities.BusinessObjects.ResourceManagement;

public class ExperimentReport : IExperimentReport
{
    public DateTime Timestamp { get; } = DateTime.UtcNow;

    public required List<IServiceInstance> RunningInstances { get; init; }

    public required List<IServiceType> ServiceTypes { get; init; }

    public required IExperimentResult ExperimentResult { get; init; }
}