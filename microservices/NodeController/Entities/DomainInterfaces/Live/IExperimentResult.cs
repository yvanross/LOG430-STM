namespace Entities.DomainInterfaces.Live;

public interface IExperimentResult
{
    double AverageLatency { get; set; }

    int ErrorCount { get; set; }

    string Message { get; set; }
}