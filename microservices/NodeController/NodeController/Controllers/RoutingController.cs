using Ambassador;
using ApplicationLogic.Extensions;
using ApplicationLogic.Services;
using ApplicationLogic.Usecases;
using Entities.BusinessObjects.Live;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NodeController.External.Docker;
using NodeController.External.Repository;

namespace NodeController.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]/[action]")]
    public class RoutingController : ControllerBase
    {
        private readonly RoutingUC _routingUc;

        private readonly HeaderService _headerService = new();

        private readonly ILogger<RoutingController> _logger;

        public RoutingController(ILogger<RoutingController> logger)
        {
            _logger = logger;

            var readModel = new PodReadModel();

            _routingUc = new(readModel);
        }

        [HttpGet]
        [ActionName(nameof(RouteByServiceType))]
        public ActionResult<IEnumerable<RoutingData>> RouteByServiceType(string caller, string serviceType, LoadBalancingMode mode)
        {
            return Ok(Try.WithConsequenceAsync(() =>
            {
                var routingDatas = _routingUc.RouteByDestinationType(caller, serviceType, mode).ToList();

                foreach (var routingData in routingDatas)
                {
                    _headerService.AddJsonHeader(routingData);

                    _headerService.AddAuthorizationHeaders(routingData, serviceType);

                    _logger.LogInformation($"routing service to {routingData.Address}");
                }

                return Task.FromResult(routingDatas);
            }, retryCount: 2));
        }
    }
}
