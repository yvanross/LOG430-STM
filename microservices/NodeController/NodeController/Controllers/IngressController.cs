using Ambassador;
using Ambassador.BusinessObjects;
using ApplicationLogic.Usecases;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NodeController.Docker;
using NodeController.Extensions;
using NodeController.Repository;

namespace NodeController.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]/{Source}/[action]")]
    public class IngressController : ControllerBase
    {
        private readonly RoutingUC _routingUc;

        private readonly MonitorUc _monitorUc;

        private readonly HeadersUC _headersUc = new();

        private readonly ILogger<SubscriptionController> _logger;

        public IngressController(ILogger<SubscriptionController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;

            var source = httpContextAccessor.HttpContext!.GetRouteValue("source")!.ToString();

            var readModel = new RepositoryRead(source);
            var writeModel = new RepositoryWrite();
            var environmentClient = new LocalDockerClient(logger);

            _routingUc = new(readModel, writeModel, environmentClient);
            _monitorUc = new(environmentClient, readModel, writeModel);
        }

        [HttpPost]
        [ActionName(nameof(HeartBeat))]
        public IActionResult HeartBeat(Guid serviceId)
        {
            try
            {
                _monitorUc.Acknowledge(serviceId);
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
                var routingDatas = _routingUc.RouteByDestinationType(serviceType, mode).ToList();

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
