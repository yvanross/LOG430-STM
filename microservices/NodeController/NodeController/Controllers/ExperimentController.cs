using ApplicationLogic.Extensions;
using ApplicationLogic.Usecases;
using Entities.DomainInterfaces.ResourceManagement;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NodeController.Dto.Incoming;
using NodeController.External.Dao;
using NodeController.External.Docker;

namespace NodeController.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]/[action]")]
    public class ExperimentController : ControllerBase
    {
        private readonly ILogger<ExperimentController> _logger;

        public ExperimentController(ILogger<ExperimentController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [ActionName(nameof(Begin))]
        public async Task<Results<Ok, BadRequest<string>>?> Begin(ExperimentDto experiment)
        {
            return await Try.WithConsequenceAsync<Results<Ok, BadRequest<string>>>(async () =>
            {
                _logger.LogInformation("Checking pool sanity...");

                if (ServicePoolDiscoveryUC.BannedIds.Any() && HostInfo.CheatsAllowed is false)
                    return TypedResults.BadRequest("Pool compromised by unregistered services, flagging as cheating");

                _logger.LogInformation("Pool sane");

                _logger.LogInformation("Creating experiment...");

                var readModel = new PodReadService();

                var experimentUc = new ChaosExperimentUC(
                    new LocalDockerClient(_logger),
                    readModel,
                    new PodWriteService(),
                    experiment.ChaosCodex,
                    new MassTransitRabbitMqClient());

                _logger.LogInformation("Experiment created");

                _logger.LogInformation("Sending experiment query to pool...");

                await experimentUc.SendTimeComparisonRequestToPool(experiment.Coordinates);

                _logger.LogInformation("Experiment sent to pool, it may begin processing");

                _logger.LogInformation("Scheduling incremental disruptions and analysis...");

                var scheduler = readModel.GetScheduler();

                scheduler.TryAddTask(nameof(experimentUc.InduceChaos), experimentUc.InduceChaos);

                _logger.LogInformation("Scheduling completed, good luck");

                return TypedResults.Ok();
            }, retryCount: 2);
        }
    }
}
