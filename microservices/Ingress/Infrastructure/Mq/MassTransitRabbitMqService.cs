using ApplicationLogic.Interfaces.Dao;
using MassTransit;
using MqContracts;
using Newtonsoft.Json;

namespace Infrastructure.Dao;

public class MassTransitRabbitMqClient<T> : IDataStream, IAckErrorEmitter<T>
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

        await Produce(routingKey, tMessage);
    }

    public async Task Produce(string routingKey, T message)
    {
        await _publishEndpoint.Publish(message,
            x => { x.SetRoutingKey(routingKey); });
    }
}