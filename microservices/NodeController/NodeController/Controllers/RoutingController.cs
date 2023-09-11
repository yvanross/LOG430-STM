using ApplicationLogic.Interfaces;
using Entities.BusinessObjects.Live;
using Entities.Dao;
using Entities.DomainInterfaces.ResourceManagement;
using Microsoft.AspNetCore.Mvc;

namespace NodeController.Controllers
{
    [ApiVersion("2.0")]
    [ApiController]
    [Route("[controller]/[action]")]
    public class RoutingController : ControllerBase
    {
        private readonly IRouting _routing;
        private readonly IPodReadService _podReadService;

        private readonly ILogger<RoutingController> _logger;

        public RoutingController(ILogger<RoutingController> logger, IRouting routing, IPodReadService podReadService)
        {
            _logger = logger;
            _routing = routing;
            _podReadService = podReadService;
        }

        [HttpGet("{serviceType}")]
        [ActionName(nameof(RouteByServiceType))]
        public ActionResult<IEnumerable<RoutingData>> RouteByServiceType([FromRoute] string serviceType, string caller, LoadBalancingMode mode)
        {
            try
            {
                var routingDatas = _routing.RouteByDestinationType(caller, serviceType, mode).ToList();

                return routingDatas.Any() ? Ok(routingDatas) : BadRequest("Service type not found");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while routing");

                return BadRequest(e.Message);
            }
        }

        [HttpPost("{serviceType}")]
        [ActionName(nameof(NegotiateSocket))]
        public ActionResult<int> NegotiateSocket([FromRoute] string serviceType)
        {
            var type = _podReadService.GetServiceType(serviceType);

            return type is null ? BadRequest("Service type not found") : Ok(_routing.NegotiateSocket(type));
        }
    }
}
