using Domain.Events.Interfaces;

namespace Domain.Events.AggregateEvents.Trip;

public record TripCreated(string TripId, HashSet<string> UpdatedScheduledStopsIds) : IDomainEvent;