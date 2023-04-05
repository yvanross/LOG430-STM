using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Dao;
using ApplicationLogic.Usecases;
using Entities.DomainInterfaces.ResourceManagement;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MqContracts;
using NodeController.Controllers;
using RabbitMQ.Client;
using HostInfo = NodeController.External.Docker.HostInfo;

namespace NodeController.External.Dao;

public class MassTransitRabbitMqClient : IDataStreamService
{
    private static IBusControl? _busControl;

    private readonly IPodReadService _podReadService = new PodReadService();

    private readonly RoutingUC _routing;

    public MassTransitRabbitMqClient()
    {
        _routing = new(_podReadService);
    }

    public async Task Produce(ICoordinates coordinates)
    {
        await BeginStreaming();

        if (_busControl is null) return;

        await _busControl.Publish(new CoordinateMessage()
        {
            StartingCoordinates = coordinates.StartingCoordinates, 
            DestinationCoordinates = coordinates.DestinationCoordinates,
        }, 
            x =>
            {
                x.SetRoutingKey("trip_comparison.query");
            });
    }

    public void EndStreaming()
    {
        _ = _busControl?.StopAsync();

        _busControl = null;
    }

    private async Task BeginStreaming()
    {
        if (_busControl is not null) return;

        var mq = _routing.RouteByDestinationType(string.Empty, HostInfo.MqServiceName, LoadBalancingMode.RoundRobin).FirstOrDefault();

        if (mq is not null)
        {
            var reformattedAddress = $"rabbitmq{mq.Address[4..]}";

            const string baseQueueName = "time_comparison.any-to-node_controller.response";

            var uniqueQueueName = $"{baseQueueName}.{Guid.NewGuid()}";

            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(reformattedAddress);

                cfg.Message<CoordinateMessage>(topologyConfigurator => topologyConfigurator.SetEntityName("coordinate_message"));
                cfg.Message<BusPositionUpdated>(topologyConfigurator => topologyConfigurator.SetEntityName("bus_position_updated"));

                cfg.ReceiveEndpoint(uniqueQueueName, endpoint =>
                {
                    endpoint.ConfigureConsumeTopology = false;

                    endpoint.Bind<BusPositionUpdated>(binding =>
                    {
                        binding.ExchangeType = ExchangeType.Topic;
                        binding.RoutingKey = "trip_comparison.response";
                    });

                    endpoint.Consumer<BusPositionUpdatedMqController>();
                });

                cfg.Publish<CoordinateMessage>(topologyConfigurator =>
                {
                    topologyConfigurator.ExchangeType = ExchangeType.Topic;
                });
            });

            _busControl = busControl;

            await busControl.StartAsync();
        }
    }
}