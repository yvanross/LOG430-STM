using Application.EventHandlers;

// MassTransit URN type resolutions, namespaces must be equal between projects for a shared type 
// ReSharper disable once CheckNamespace
namespace Contracts;

public class ApplicationRideTrackingUpdated : Event
{
    public ApplicationRideTrackingUpdated(string message, bool trackingCompleted, double duration, Guid id, DateTime created) : base(id, created)
    {
        Message = message;
        TrackingCompleted = trackingCompleted;
        Duration = duration;
    }

    public string Message { get; set; }

    public bool TrackingCompleted { get; set; }

    public double Duration { get; set; }
}