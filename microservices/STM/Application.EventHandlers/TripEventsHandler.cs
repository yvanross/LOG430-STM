using Application.EventHandlers.AntiCorruption;
using Domain.Events.AggregateEvents.Trip;
using Domain.Events.Interfaces;

namespace Application.EventHandlers;

public class TripEventsHandler : IDomainEventHandler<TripScheduledStopsUpdated>, IDomainEventHandler<TripCreated>
{
    private readonly IPublisher _publisher;

    public TripEventsHandler(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task HandleAsync(TripScheduledStopsUpdated domainEvent)
    {
        await _publisher.Publish(domainEvent);
    }

    public async Task HandleAsync(TripCreated domainEvent)
    {
        await _publisher.Publish(domainEvent);
    }
}