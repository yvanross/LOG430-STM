namespace Entities.DomainInterfaces;

public interface IBusTracking
{
    string Message { get; set; }

    bool TrackingCompleted { get; set; }

    int Duration { get; set; }
}