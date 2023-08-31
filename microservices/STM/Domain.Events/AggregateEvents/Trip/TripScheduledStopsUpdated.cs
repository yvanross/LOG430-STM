using Domain.Events.Interfaces;

namespace Domain.Events.AggregateEvents.Trip;

public record TripScheduledStopsUpdated(string TripId, HashSet<string> UpdatedScheduledStopsIds) : IDomainEvent { }