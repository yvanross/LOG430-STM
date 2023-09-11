using ApplicationLogic.Interfaces;
using ApplicationLogic.Usecases;
using Entities.Dao;
using Infrastructure.Interfaces;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using MqContracts;
using NodeController.Controllers.Mq;
using RabbitMQ.Client;

namespace Configuration;

public class MassTransitRabbitMq : IMqConfigurator
{
    public IBusControl? BusControl;

    private readonly IRouting _routing;
    private readonly IHostInfo _hostInfo;
    private readonly IServiceProvider _serviceProvider;
    private readonly IPodReadService _podReadService;

    public MassTransitRabbitMq(IRouting routing, IHostInfo hostInfo, IServiceProvider serviceProvider, IPodReadService podReadService)
    {
        _routing = routing;
        _hostInfo = hostInfo;
        _serviceProvider = serviceProvider;
        _podReadService = podReadService;
    }

    public IBusControl GetPublishEndpoint()
    {
        if (BusControl is null) throw new ArgumentNullException(nameof(BusControl));

        return BusControl;
    }

    public async Task Configure()
    {
        if (_hostInfo.IsIngressConfigValid() is false) return;

        if (BusControl is not null && BusControl.CheckHealth().Status.Equals(BusHealthStatus.Unhealthy))
        {
            await BusControl.StopAsync();

            BusControl = null;
        }

        if (BusControl is not null) return;

        var reformattedAddress = $"rabbitmq://{_hostInfo.GetAddress()}:{_routing.NegotiateSocket(_podReadService.GetServiceType(_hostInfo.GetMQServiceName())!)}";

        const string baseQueueName = "time_comparison.any-to-node_controller.response";

        var uniqueQueueName = $"{baseQueueName}.{Guid.NewGuid()}";

        var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.Host(reformattedAddress, c =>
            {
                c.RequestedConnectionTimeout(100);
                c.Heartbeat(TimeSpan.FromMilliseconds(50));
            });

            cfg.Message<CoordinateMessage>(topologyConfigurator => topologyConfigurator.SetEntityName("coordinate_message"));

            cfg.Message<BusPositionUpdated>(topologyConfigurator => topologyConfigurator.SetEntityName("bus_position_updated"));

            cfg.ReceiveEndpoint(uniqueQueueName, endpoint =>
            {
                endpoint.ConfigureConsumeTopology = false;
                
                endpoint.SetQuorumQueue(_podReadService.GetPodType(_hostInfo.GetMQServiceName())!.NumberOfInstances);

                endpoint.PrefetchCount = 1;

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