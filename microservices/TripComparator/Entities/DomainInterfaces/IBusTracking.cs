namespace Entities.DomainInterfaces;

public interface IBusTracking
{
    string Message { get; set; }

    bool TrackingCompleted { get; set; }

    double Duration { get; set; }
}