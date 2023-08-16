using Domain.Events.Interfaces;

namespace Domain.Events.AggregateEvents.Ride;

public record RideRegisteredSuccessfully(string Id) : IDomainEvent;