using Domain.Events.Interfaces;

namespace Domain.Events.AggregateEvents.Ride;

public record struct BusTrackingUpdated(string Message, bool TrackingCompleted, double Duration) : IDomainEvent;