using ApplicationLogic.Usecases;
using Ingress.Extensions;
using Ingress.Repository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Ingress.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]/[action]")]
    public class IngressController : ControllerBase
    {
        private readonly RoutingUC _routingUc;

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
