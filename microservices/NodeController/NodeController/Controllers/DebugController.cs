using ApplicationLogic.Extensions;
using ApplicationLogic.Services;
using ApplicationLogic.Usecases;
using Entities.BusinessObjects.Live;
using Entities.BusinessObjects.ResourceManagement;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;
using Entities.DomainInterfaces.ResourceManagement;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NodeController.Cache;
using NodeController.External.Dao;

namespace NodeController.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]/[action]")]
    public class DebugController : ControllerBase
    {
        private readonly ILogger<DebugController> _logger;

        public DebugController(ILogger<DebugController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ActionName(nameof(GetPodTypes))]
        public List<IPodType> GetPodTypes()
        {
            return RouteCache.GetPodTypes().Values.ToList();
        }

        [HttpGet]
        [ActionName(nameof(GetPodInstances))]
        public List<IPodInstance> GetPodInstances()
        {
            return RouteCache.GetPodInstances().ToList();
        }
        
        [HttpGet]
        [ActionName(nameof(CassandraPost))]
        public async Task<IActionResult> CassandraPost()
        {
            var cwm = new InfluxDbWriteService();

            await cwm.Log(new ExperimentReport()
            {
                ExperimentResult = new ExperimentResult()
                {
                    Message = "testMessage",
                    AverageLatency = 4,
                    ErrorCount = 2,
                },
                RunningInstances = new List<IServiceInstance>(),
                ServiceTypes = new List<IServiceType>()
            });

            return Ok();
        }
    }
}
