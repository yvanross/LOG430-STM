using Entities.DomainInterfaces.Live;

namespace Entities.BusinessObjects.Live;

public class ExperimentResult : IExperimentResult
{
    public required double AverageLatency { get; set; }

    public required int ErrorCount { get; set; }
    
    public required string Message { get; set; } = string.Empty;
}