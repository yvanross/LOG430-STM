using ApplicationLogic.Extensions;
using ApplicationLogic.Usecases;
using Entities.BusinessObjects.Live;
using Entities.DomainInterfaces.ResourceManagement;
using Microsoft.AspNetCore.Mvc;

namespace NodeController.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoutingController : ControllerBase
    {
        private readonly Routing _routing;

        private readonly ILogger<RoutingController> _logger;

        public RoutingController(ILogger<RoutingController> logger, Routing routing)
        {
            _logger = logger;
            _routing = routing;
        }

        [HttpGet("{serviceType}")]
        [ActionName(nameof(RouteByServiceType))]
        public ActionResult<IEnumerable<RoutingData>> RouteByServiceType([FromRoute] string serviceType, string caller, LoadBalancingMode mode)
        {
            return Ok(Try.WithConsequenceAsync(() =>
            {
                var routingDatas = _routing.RouteByDestinationType(caller, serviceType, mode).ToList();

                return Task.FromResult(routingDatas);
            }, retryCount: 2).Result);
        }
    }
}
