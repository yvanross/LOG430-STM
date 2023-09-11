using Domain.Common.Interfaces.Events;

namespace Domain.Aggregates.Ride.Events;

public readonly record struct RideTrackingUpdated(string Message, bool TrackingCompleted, double Duration) : IDomainEvent;