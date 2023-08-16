using Domain.Events.Interfaces;

namespace Domain.Events.AggregateEvents.Trip;

public record struct TripScheduledStopsUpdated(string TripId) : IDomainEvent;