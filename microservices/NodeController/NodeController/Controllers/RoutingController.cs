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
                var routingDatas = _routing.RouteByDestinationType(caller, serviceType, mode).ToList();

                return Task.FromResult(routingDatas);
            }, retryCount: 2).Result);
        }
    }
}
