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

            _routingUC = new(readModel);
            _monitorUc = new(new LocalDockerClient(), new HeartBeatService(readModel, new RepositoryWrite(), new LocalDockerClient()));
        }

        [HttpPut]
        [ActionName(nameof(HeartBeat))]
        public async Task<IActionResult> HeartBeat(Guid serviceId)
        {
            try
            {

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
        public ActionResult<RoutingData> RouteByServiceType(string serviceType)
        {
            return Try.WithConsequence(() =>
            {
                var address = _routingUC.RouteByDestinationType(serviceType);

                var routingData = new RoutingData() { Address = address };

                _headersUc.AddJsonHeader(routingData);

                _headersUc.AddAuthorizationHeaders(routingData, serviceType);

                _logger.LogInformation($"routing service to {routingData.Address}");

                return routingData;
            });
        }
    }
}
