using ApplicationLogic.Interfaces.Dao;
using MassTransit;
using Newtonsoft.Json;

namespace Infrastructure.Dao;

public class MassTransitRabbitMqClient<T> : IDataStream
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MassTransitRabbitMqClient(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task Produce(string routingKey, string message)
    {
        var tMessage = JsonConvert.DeserializeObject<T>(message);

        if (tMessage is null) throw new NullReferenceException("Deserialized message was null");

        await _publishEndpoint.Publish(
            tMessage,
            x => { x.SetRoutingKey(routingKey); });
    }
}