using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Dao;
using Infrastructure.Interfaces;
using MassTransit;
using MqContracts;

namespace Infrastructure.Dao;

public class MassTransitRabbitMqClient : IDataStreamService, IHeartbeatService
{
    private readonly IMqConfigurator _configurator;
    private readonly IPublishEndpoint _portainerRabbitMqEndpoint;

    public MassTransitRabbitMqClient(IMqConfigurator configurator, IPublishEndpoint portainerRabbitMqEndpoint)
    {
        _configurator = configurator;
        _portainerRabbitMqEndpoint = portainerRabbitMqEndpoint;
    }

    public async Task Produce(ICoordinates coordinates)
    {
        await _configurator.Configure();

        await _configurator.GetPublishEndpoint().Publish(new CoordinateMessage()
        {
            StartingCoordinates = coordinates.StartingCoordinates, 
            DestinationCoordinates = coordinates.DestinationCoordinates,
        }, 
            x =>
            {
                x.SetRoutingKey("trip_comparison.query");
            });
    }

    public async Task Produce(HeartBeatDto heartBeatDto)
    {
        try
        {
            await _portainerRabbitMqEndpoint.Publish(heartBeatDto,
                x =>
                {
                    x.SetRoutingKey("heartBeat_event");
                });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}