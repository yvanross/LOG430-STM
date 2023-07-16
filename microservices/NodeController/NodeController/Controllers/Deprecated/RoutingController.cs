using ApplicationLogic.Interfaces;
using Entities.BusinessObjects.Live;
using Entities.DomainInterfaces.ResourceManagement;
using Entities.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace NodeController.Controllers
{
    [ApiVersion("1.0", Deprecated = true)]
    [ApiController]
    [Route("[controller]")]
    public class RoutingControllerV1 : ControllerBase
    {
        private readonly IRouting _routing;

        private readonly ILogger<RoutingController> _logger;

        public RoutingControllerV1(ILogger<RoutingController> logger, IRouting routing)
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
