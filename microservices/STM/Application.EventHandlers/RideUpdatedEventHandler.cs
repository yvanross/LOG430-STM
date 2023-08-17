using Application.EventHandlers.AntiCorruption;
using Domain.Events.AggregateEvents.Ride;
using Domain.Events.Interfaces;

namespace Application.EventHandlers;

public class RideUpdatedEventHandler : IDomainEventHandler<BusTrackingUpdated>
{
    private readonly IPublisher _publisher;

    public RideUpdatedEventHandler(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task HandleAsync(BusTrackingUpdated domainEvent)
    {
        //decoupling the domain event from the infrastructure
        await _publisher.Publish(
            new Contracts.BusTrackingUpdated(
                domainEvent.Message, 
                domainEvent.TrackingCompleted, 
                domainEvent.Duration));
    }
}