using ApplicationLogic.Interfaces;
using Entities.BusinessObjects.Live;
using Entities.DomainInterfaces.ResourceManagement;
using Entities.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace NodeController.Controllers.Deprecated
{
    [ApiVersion("1.0", Deprecated = true)]
    [ApiController]
    [Route("[controller]")]
    public class RoutingController : ControllerBase
    {
        private readonly IRouting _routing;

        private readonly ILogger<Controllers.RoutingController> _logger;

        public RoutingController(ILogger<Controllers.RoutingController> logger, IRouting routing)
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
