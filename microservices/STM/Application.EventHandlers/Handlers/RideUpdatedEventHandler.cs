using Application.CommandServices.Repositories;
using Application.EventHandlers.AntiCorruption;
using Contracts;
using Domain.Common.Interfaces;
using Domain.Events.AggregateEvents.Ride;
using Domain.Events.Interfaces;

namespace Application.EventHandlers.Handlers;

public class RideUpdatedEventHandler : IDomainEventHandler<BusTrackingUpdated>
{
    private readonly IPublisher _publisher;
    private readonly IEventContext _eventContext;
    private readonly IDatetimeProvider _datetimeProvider;

    public RideUpdatedEventHandler(IPublisher publisher, IEventContext eventContext, IDatetimeProvider datetimeProvider)
    {
        _publisher = publisher;
        _eventContext = eventContext;
        _datetimeProvider = datetimeProvider;
    }

    public async Task HandleAsync(BusTrackingUpdated domainEvent)
    {
        var applicationEvent = new RideTrackingUpdated(
            domainEvent.Message,
            domainEvent.TrackingCompleted,
            domainEvent.Duration,
            Guid.NewGuid(),
            _datetimeProvider.GetCurrentTime());

        //decoupling the domain event from the infrastructure
        await _publisher.Publish(applicationEvent);

        await _eventContext.AddOrUpdateAsync(applicationEvent);
    }
}