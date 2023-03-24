namespace Entities.DomainInterfaces.Live;

public interface ITestSnapshot
{
    double AverageLatency { get; set; }

    int ErrorCount { get; set; }

    string Message { get; set; }
}