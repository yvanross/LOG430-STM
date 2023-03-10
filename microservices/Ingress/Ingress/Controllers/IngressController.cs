using Ambassador;
using Ambassador.BusinessObjects;
using ApplicationLogic.Services;
using ApplicationLogic.Usecases;
using Ingress.Extensions;
using Ingress.Repository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Monitor.Docker;

namespace Ingress.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]/{source}/[action]")]
    public class IngressController : ControllerBase
    {
        private readonly RoutingUC _routingUC;

        private readonly MonitorUc _monitorUc;

        private readonly HeadersUC _headersUc = new();

        private readonly ILogger<SubscriptionController> _logger;

        public IngressController(ILogger<SubscriptionController> logger, string source)
        {
            _logger = logger;

            var readModel = new RepositoryRead(source);
            var writeModel = new RepositoryWrite();
            var environmentClient = new LocalDockerClient();

            _routingUC = new(readModel, environmentClient);
            _monitorUc = new(environmentClient, readModel, writeModel);
        }

        [HttpPut]
        [ActionName(nameof(HeartBeat))]
        public async Task<IActionResult> HeartBeat(Guid serviceId)
        {
            try
            {
                _monitorUc.ReceiveHeartBeat(serviceId);
            }
            catch (Exception e)
            {
                var errorMessage = $"{e.Message} \n \t{e.StackTrace}";

                _logger.LogError(errorMessage);

                return Problem(errorMessage);
            }

            return Ok();
        }

        [HttpGet]
        [ActionName(nameof(RouteByServiceType))]
        public ActionResult<IEnumerable<RoutingData>> RouteByServiceType(string serviceType, LoadBalancingMode mode)
        {
            return Try.WithConsequence(() =>
            {
                var routingDatas = _routingUC.RouteByDestinationType(serviceType, mode).ToList();

                foreach (var routingData in routingDatas)
                {
                    _headersUc.AddJsonHeader(routingData);

                    _headersUc.AddAuthorizationHeaders(routingData, serviceType);

                    _logger.LogInformation($"routing service to {routingData.Address}");
                }

                return routingDatas;
            });
        }
    }
}
