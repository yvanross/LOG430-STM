using System.Collections.Concurrent;
using System.Collections.Immutable;
using Ambassador;
using ApplicationLogic.Interfaces.Dao;
using ApplicationLogic.Usecases;
using Entities.BusinessObjects.Live;
using Entities.DomainInterfaces.Live;
using MassTransit;
using NodeController.External.Docker;
using HostInfo = NodeController.External.Docker.HostInfo;
using ISaga = Entities.DomainInterfaces.Live.ISaga;

namespace NodeController.External.Repository;

public class MassTransitRabbitMqClient : IDataStreamReadModel, IConsumer<Saga>
{
    private readonly IPodReadModel _podReadModel = new PodReadModel();

    private readonly RoutingUC _routing;

    private readonly ChaosExperimentUC _chaosExperimentUc;

    private IBusControl? _busControl;

    public MassTransitRabbitMqClient()
    {
        _routing = new(_podReadModel);
        _chaosExperimentUc = new ChaosExperimentUC(new LocalDockerClient(null), _podReadModel, new PodWriteModel());
    }

    public void BeginStreaming()
    {
        var mq = _routing.RouteByDestinationType(string.Empty, HostInfo.MqServiceName, LoadBalancingMode.RoundRobin).FirstOrDefault();

        if (mq is not null)
        {
            var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
            {
                cfg.Host(new Uri(mq.Address));

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

    public async Task EndStreaming()
    {
        _ = _busControl?.StopAsync();
    }

    public Task Consume(ConsumeContext<Saga> context)
    {
        return _chaosExperimentUc.ReportTestResult(context.Message);
    }
}