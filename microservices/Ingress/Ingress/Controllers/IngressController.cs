using ApplicationLogic.Usecases;
using CommunicatorNuget.BusinessObjects;
using Ingress.Repository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using CommunicatorNuget.Usecases;

namespace Ingress.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]/[action]")]
    public class IngressController : ControllerBase
    {
        private readonly SubscriptionUC _subscriptionUC = new (new RepositoryWrite(), new RepositoryRead());

        private readonly RoutingUC _routingUC = new (new RepositoryRead());

        private readonly ILogger<IngressController> _logger;

        public IngressController(ILogger<IngressController> logger)
        {
            _logger = logger;
        }

        [HttpPut]
        [ActionName(nameof(Subscribe))]
        public IActionResult Subscribe(string serviceType, string serviceName = "")
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            if (ip is null)
                return UnprocessableEntity("No Remote Ip Address");

            var port = HttpContext.Connection.RemotePort.ToString();

            _subscriptionUC.Subscribe(serviceName, ip, port, serviceType);

            return Ok();
        }

        [HttpGet]
        [ActionName(nameof(Route))]
        public ActionResult<RouteTarget> RouteByServiceType(string serviceType)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            if (ip is null)
                return UnprocessableEntity("No Remote Ip Address");

            var port = HttpContext.Connection.RemotePort.ToString();

            if(_subscriptionUC.CheckIfServiceIsSubscribed(ip, port))
                return Unauthorized("Subscribe your service before making routing requests");

            return Ok();
        }
    }
}