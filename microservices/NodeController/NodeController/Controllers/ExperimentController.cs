using ApplicationLogic.Extensions;
using ApplicationLogic.Usecases;
using Entities.DomainInterfaces.ResourceManagement;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
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
        [ActionName(nameof(BeginExperiment))]
        public IActionResult BeginExperiment(IChaosCodex chaosCodex)
        {
            Try.WithConsequenceAsync(() =>
            {
                _logger.LogInformation("Scheduling experiment...");

                var readModel = new PodReadModel();

                var scheduler = readModel.GetScheduler();

                var experimentUc = new ChaosExperimentUC(
                    new LocalDockerClient(_logger),
                    readModel,
                    new PodWriteModel(),
                    chaosCodex,
                    new CassandraWriteModel(),
                    new MassTransitRabbitMqClient());

                scheduler.TryAddTask(nameof(experimentUc.InduceChaos), experimentUc.InduceChaos);

                _logger.LogInformation("Experiment scheduled");

                return Task.FromResult(0);
            }, retryCount: 2).Wait();

            return Ok();
        }
    }
}
