using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Policies;
using Entities.DomainInterfaces;
using MassTransit;
using MqContracts;

namespace Infrastructure.Clients;

public class MassTransitRabbitMqClient : IDataStreamWriteModel
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IBackOffRetryPolicy<MassTransitRabbitMqClient> _backOffRetry;

    public MassTransitRabbitMqClient(IPublishEndpoint publishEndpoint, IBackOffRetryPolicy<MassTransitRabbitMqClient> backOffRetry)
    {
        _publishEndpoint = publishEndpoint;
        _backOffRetry = backOffRetry;
    }

    public async Task Produce(IBusPositionUpdated busPositionUpdated)
    {
        await _backOffRetry.ExecuteAsync(async () =>
        {
            await _publishEndpoint.Publish(new BusPositionUpdated()
                {
                    Message = busPositionUpdated.Message,
                    Seconds = busPositionUpdated.Seconds,
                },
                x =>
                {
                    x.SetRoutingKey("trip_comparison.response");
                });
        }).ConfigureAwait(false);
    }
}