using System.Collections.Concurrent;
using System.Collections.Immutable;
using Ambassador;
using ApplicationLogic.Interfaces.Dao;
using ApplicationLogic.Usecases;
using Entities.DomainInterfaces.Live;
using MassTransit;
using HostInfo = NodeController.External.Docker.HostInfo;
using ISaga = Entities.DomainInterfaces.Live.ISaga;

namespace NodeController.External.Repository;

public class MassTransitRabbitMqClient : IDataStreamReadModel, IConsumer<ISaga>
{

    ImmutableHashSet<string> _dataStreams;

    private RoutingUC routing = new RoutingUC(new PodReadModel(HostInfo.ServiceAddress));

    public void BeginStreaming(string testId)
    {
        if (_dataStreams.Contains(testId) is false)
        {
            var mq = routing.RouteByDestinationType(string.Empty, HostInfo.MqServiceName, LoadBalancingMode.RoundRobin).FirstOrDefault();

            if (mq is not null)
            {
                var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
                {
                    cfg.Host(new Uri(mq.Address));

                    cfg.ReceiveEndpoint(testId, e =>
                    {
                        e.Consumer<MassTransitRabbitMqClient>();
                    });
                });

                _ = busControl.StartAsync();
            }
        }
    }

    public Task Consume(ConsumeContext<ISaga> context)
    {
        var 
    }
}