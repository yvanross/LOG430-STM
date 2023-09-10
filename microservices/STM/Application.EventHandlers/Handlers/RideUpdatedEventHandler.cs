using Application.EventHandlers.Interfaces;
using Contracts;
using Domain.Aggregates.Ride.Events;
using Domain.Common.Interfaces;
using Domain.Common.Interfaces.Events;

namespace Application.EventHandlers.Handlers;

public class RideUpdatedEventHandler : IDomainEventHandler<RideTrackingUpdated>
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

    public async Task HandleAsync(RideTrackingUpdated domainEvent)
    {
        var applicationEvent = new ApplicationRideTrackingUpdated(
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