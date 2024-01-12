using Application.Common.Interfaces.Policies;
using Application.EventHandlers.Interfaces;
using MassTransit;
using Event = Application.EventHandlers.Event;

namespace Infrastructure.TcpClients;

public class MassTransitPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IInfiniteRetryPolicy<MassTransitPublisher> _retryPolicy;

    public MassTransitPublisher(IPublishEndpoint publishEndpoint,
        IInfiniteRetryPolicy<MassTransitPublisher> retryPolicy)
    {
        _publishEndpoint = publishEndpoint;
        _retryPolicy = retryPolicy;
    }

    public async Task Publish<TEvent>(TEvent message) where TEvent : Event
    {
        await _retryPolicy.ExecuteAsync(async () => await _publishEndpoint.Publish(message,
            x =>
            {
                x.SetRoutingKey("Stm.RideTrackingUpdated");
            },
            new CancellationTokenSource(TimeSpan.FromMilliseconds(50)).Token));
    }
}