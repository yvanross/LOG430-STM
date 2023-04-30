using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Dao;
using ApplicationLogic.Usecases;
using Entities.DomainInterfaces.ResourceManagement;
using Infrastructure.Interfaces;
using MassTransit;
using MassTransit.Transports;
using MqContracts;
using RabbitMQ.Client;

namespace Infrastructure.Dao;

public class MassTransitRabbitMqClient : IDataStreamService
{
    private readonly IMqConfigurator _configurator;

    public MassTransitRabbitMqClient(IMqConfigurator configurator)
    {
        _configurator = configurator;
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
}