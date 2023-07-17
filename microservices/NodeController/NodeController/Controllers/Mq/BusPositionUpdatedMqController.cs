using ApplicationLogic.Services;
using ApplicationLogic.Usecases;
using MassTransit;
using MqContracts;

namespace NodeController.Controllers.Mq;

public class BusPositionUpdatedMqController : IConsumer<BusPositionUpdated>
{
    private readonly ExperimentMonitoring _experimentMonitoring;

    public BusPositionUpdatedMqController(ExperimentMonitoring experimentMonitoring)
    {
        _experimentMonitoring = experimentMonitoring;
    }

    public Task Consume(ConsumeContext<BusPositionUpdated> context)
    {
        _experimentMonitoring.AnalyzeAndStoreRealtimeTestData(context.Message);

        return Task.CompletedTask;
    }
}