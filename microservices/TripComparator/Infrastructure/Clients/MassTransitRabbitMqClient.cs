using ApplicationLogic.Interfaces;
using Entities.DomainInterfaces;
using MassTransit;
using MqContracts;

namespace TripComparator.External;

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
            });
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
        }
    }
}