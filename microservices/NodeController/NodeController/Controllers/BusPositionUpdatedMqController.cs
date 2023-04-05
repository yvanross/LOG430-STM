using ApplicationLogic.Usecases;
using MassTransit;
using MqContracts;
using NodeController.External.Dao;

namespace NodeController.Controllers;

public class BusPositionUpdatedMqController : IConsumer<BusPositionUpdated>
{
    private readonly ExperimentMonitoringUC _experimentMonitoringUc;

    private readonly PodReadService _readService;

    public BusPositionUpdatedMqController()
    {
        _readService = new PodReadService();
        _experimentMonitoringUc = new(new InfluxDbWriteService(), _readService);
    }

    public Task Consume(ConsumeContext<BusPositionUpdated> context)
    {
        _experimentMonitoringUc.AnalyzeAndStoreRealtimeTestData(context.Message);

        _readService.GetScheduler().TryAddTask(nameof(_experimentMonitoringUc.LogExperimentResults), _experimentMonitoringUc.LogExperimentResults);

        return Task.CompletedTask;
    }
}