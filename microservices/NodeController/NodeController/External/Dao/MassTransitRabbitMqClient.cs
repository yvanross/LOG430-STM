using ApplicationLogic.Interfaces.Dao;
using ApplicationLogic.Usecases;
using Entities.DomainInterfaces.ResourceManagement;
using MassTransit;
using NodeController.Dto;
using HostInfo = NodeController.External.Docker.HostInfo;
using ISaga = Entities.DomainInterfaces.Live.ISaga;

namespace NodeController.External.Dao;

public class MassTransitRabbitMqClient : IDataStreamReadModel, IConsumer<Saga>
{
    private readonly IPodReadModel _podReadModel = new PodReadModel();

    private readonly RoutingUC _routing;

    private IBusControl? _busControl;

    private Action<ISaga>? _reportTestResult;

    public MassTransitRabbitMqClient()
    {
        _routing = new(_podReadModel);
    }

    public void BeginStreaming(Action<ISaga> reportTestResult)
    {
        var mq = _routing.RouteByDestinationType(string.Empty, HostInfo.MqServiceName, LoadBalancingMode.RoundRobin).FirstOrDefault();

        if (mq is not null)
        {
            _reportTestResult = reportTestResult;
            
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host($"rabbitmq{mq.Address[4..]}", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint("TimeComparison", e =>
                {
                    e.PrefetchCount = 1;

                    e.Consumer<MassTransitRabbitMqClient>();
                });
            });

            _busControl = busControl;

            _ = busControl.StartAsync();
        }
    }

    public void EndStreaming()
    {
        _ = _busControl?.StopAsync();
    }

    public Task Consume(ConsumeContext<Saga> context)
    {
        _reportTestResult?.Invoke(context.Message);

        return Task.CompletedTask;
    }
}