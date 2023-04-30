using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Dao;
using ApplicationLogic.Usecases;
using Entities.DomainInterfaces.ResourceManagement;
using MassTransit;
using MqContracts;

namespace NodeController.Controllers;

public class ExperimentMqController : IConsumer<ExperimentDto>
{
    private readonly ChaosExperiment _chaosExperiment;
    private readonly IScheduler _scheduler;
    private readonly ILogger<ExperimentMqController> _logger;
    private readonly IHostInfo _hostInfo;

    public ExperimentMqController(ChaosExperiment chaosExperiment, IScheduler scheduler, ILogger<ExperimentMqController> logger, IHostInfo hostInfo)
    {
        _chaosExperiment = chaosExperiment;
        _scheduler = scheduler;
        _logger = logger;
        _hostInfo = hostInfo;
    }

    public async Task Consume(ConsumeContext<ExperimentDto> context)
    {
        var experiment = context.Message;

        _logger.LogInformation("Checking pool sanity...");

        if (ServicePoolDiscovery.BannedIds.Any() && _hostInfo.GetCheatsAllowed() is false)
        {
            _logger.LogCritical("Pool compromised by unregistered services, flagging as cheating");

            return;
        }

        _logger.LogInformation("Pool sane");

        _logger.LogInformation("Creating experiment...");

        _chaosExperiment.SetChaosCodex(experiment.ChaosCodex);

        await _chaosExperiment.SendTimeComparisonRequestToPool(experiment.Coordinates);

        _logger.LogInformation("Experiment sent to pool, it may begin processing");

        _logger.LogInformation("Scheduling incremental disruptions and analysis...");

        //_scheduler.TryAddTask(nameof(_chaosExperiment.InduceChaos), _chaosExperiment.InduceChaos);

        _logger.LogInformation("Scheduling completed, good luck");
    }
}