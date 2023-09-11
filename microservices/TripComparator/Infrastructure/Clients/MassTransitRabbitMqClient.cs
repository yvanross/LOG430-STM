using Application.Interfaces;
using MassTransit;
using MqContracts;

namespace Infrastructure.Clients;

public class MassTransitRabbitMqClient : IDataStreamWriteModel
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MassTransitRabbitMqClient(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task Produce(IBusPositionUpdated busPositionUpdated)
    {
        try
        {
            await _publishEndpoint.Publish(new BusPositionUpdated()
            {
                Message = busPositionUpdated.Message,
                Seconds = busPositionUpdated.Seconds,
            },
            x =>
            {
                x.SetRoutingKey("trip_comparison.response");
            }, new CancellationTokenSource(TimeSpan.FromMilliseconds(100)).Token);
        }
        catch
        {
            // ignored - no need to fight over ack - single message is not that important in our context
        }
    }
}