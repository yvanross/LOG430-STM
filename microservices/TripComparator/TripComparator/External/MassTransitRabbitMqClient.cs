using Ambassador;
using Ambassador.Controllers;
using ApplicationLogic.Interfaces;
using MassTransit;
using ISaga = Entities.DomainInterfaces.ISaga;

namespace TripComparator.External;

public class MassTransitRabbitMqClient : IDataStreamWriteModel
{
    private IBusControl? _busControl;

    public async Task BeginStreaming()
    {
        var mq = (await RestController.GetAddress(HostInfo.MqServiceName, LoadBalancingMode.RoundRobin)).FirstOrDefault();

        if (mq is not null)
        {
            var reformattedAddress = $"rabbitmq{mq.Address[4..]}";

            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(reformattedAddress, h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
            });

            _busControl = busControl;

            try
            {
                _ = await busControl.StartAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    public void CloseChannel()
    {
        _ = _busControl?.StopAsync();

        _busControl = null;
    }

    public async Task Produce(ISaga saga)
    {
        var sendEndpoint = await _busControl!.GetSendEndpoint(new Uri("queue:TimeComparison"));

        await sendEndpoint.Send(saga);
    }
}