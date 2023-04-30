using ApplicationLogic.Interfaces.Dao;
using ApplicationLogic.Usecases;
using Entities.DomainInterfaces.ResourceManagement;
using MassTransit;
using MqContracts;

namespace NodeController.Controllers;

public class BusPositionUpdatedMqController : IConsumer<BusPositionUpdated>
{
    private readonly ExperimentMonitoring _experimentMonitoring;
    private readonly IScheduler _scheduler;

    public BusPositionUpdatedMqController(ExperimentMonitoring experimentMonitoring, IScheduler scheduler)
    {
        _experimentMonitoring = experimentMonitoring;
        _scheduler = scheduler;
    }

    public Task Consume(ConsumeContext<BusPositionUpdated> context)
    {
        _experimentMonitoring.AnalyzeAndStoreRealtimeTestData(context.Message);

        _scheduler.TryAddTask(nameof(_experimentMonitoring.LogExperimentResults), _experimentMonitoring.LogExperimentResults);

        return Task.CompletedTask;
    }
}