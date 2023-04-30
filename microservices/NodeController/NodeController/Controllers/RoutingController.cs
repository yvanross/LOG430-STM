using ApplicationLogic.Extensions;
using ApplicationLogic.Services;
using ApplicationLogic.Usecases;
using Entities.BusinessObjects.Live;
using Entities.DomainInterfaces.ResourceManagement;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace NodeController.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class RoutingController : ControllerBase
    {
        private readonly Routing _routing;

        private readonly ILogger<RoutingController> _logger;

        public RoutingController(ILogger<RoutingController> logger, Routing routing)
        {
            _logger = logger;
            _routing = routing;
        }

        [HttpGet]
        [ActionName(nameof(RouteByServiceType))]
        public ActionResult<IEnumerable<RoutingData>> RouteByServiceType(string caller, string serviceType, LoadBalancingMode mode)
        {
            return Ok(Try.WithConsequenceAsync(() =>
            {
                _logger.LogInformation($"Finding best route for {caller} to {serviceType} in {Enum.GetName(mode)} mode");

                var routingDatas = _routing.RouteByDestinationType(caller, serviceType, mode).ToList();

                foreach (var routingData in routingDatas)
                {
                    _logger.LogInformation($"routing service to {routingData.Address}");
                }

                return Task.FromResult(routingDatas);
            }, retryCount: 2).Result);
        }
    }
}
