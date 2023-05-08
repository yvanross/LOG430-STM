using ApplicationLogic.Interfaces;
using ApplicationLogic.Usecases;
using Entities.DomainInterfaces.ResourceManagement;
using Infrastructure.Interfaces;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using MqContracts;
using NodeController.Controllers;
using RabbitMQ.Client;

namespace Configuration;

public class MassTransitRabbitMq : IMqConfigurator
{
    public IBusControl? BusControl;

    private readonly Routing _routing;
    private readonly IHostInfo _hostInfo;
    private readonly IServiceProvider _serviceProvider;

    public MassTransitRabbitMq(Routing routing, IHostInfo hostInfo, IServiceProvider serviceProvider)
    {
        _routing = routing;
        _hostInfo = hostInfo;
        _serviceProvider = serviceProvider;
    }

    public IBusControl GetPublishEndpoint()
    {
        if(BusControl is null) throw new ArgumentNullException(nameof(BusControl));

        return BusControl;
    }

    public async Task Configure()
    {
        if (BusControl is not null && BusControl.CheckHealth().Status.Equals(BusHealthStatus.Unhealthy))
        {
            await BusControl.StopAsync();

            BusControl = null;
        }

        if (BusControl is not null) return;

        var mq = _routing.RouteByDestinationType(string.Empty, _hostInfo.GetMQServiceName(), LoadBalancingMode.RoundRobin).FirstOrDefault();

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

                    endpoint.Consumer(typeof(BusPositionUpdatedMqController), (_) => new BusPositionUpdatedMqController(_serviceProvider.GetRequiredService<ExperimentMonitoring>()));

                    endpoint.Bind<BusPositionUpdated>(binding =>
                    {
                        binding.ExchangeType = ExchangeType.Topic;
                        binding.RoutingKey = "trip_comparison.response";
                    });
                });

                cfg.Publish<CoordinateMessage>(topologyConfigurator =>
                {
                    topologyConfigurator.ExchangeType = ExchangeType.Topic;
                });
            });

            BusControl = busControl;

            await busControl.StartAsync();
        }
    }
}