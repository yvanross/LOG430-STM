using Domain.Events.Interfaces;

namespace Domain.Events.AggregateEvents.Trip;

public readonly record struct TripCreated(string TripId, params string[] UpdatedScheduledStopsIds) : IDomainEvent;