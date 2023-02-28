using Ambassador.BusinessObjects;
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
        private readonly SubscriptionUC _subscriptionUC = new (new RepositoryWrite(), new RepositoryRead());

        private readonly RoutingUC _routingUC = new (new RepositoryRead());
        
        private readonly HeadersUC _headersUc = new ();

        private readonly ILogger<IngressController> _logger;

        public IngressController(ILogger<IngressController> logger)
        {
            _logger = logger;
        }

        [HttpPut]
        [ActionName(nameof(Subscribe))]
        public IActionResult Subscribe(string serviceType, string serviceName = "")
        {
            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

                if (ip is null)
                    return UnprocessableEntity("No Remote Ip Address");

                var port = HttpContext.Connection.RemotePort.ToString();

                _subscriptionUC.Subscribe(serviceName, ip, port, serviceType);

                _logger.LogInformation($"{serviceType} from {ip}:{port} has subscribed");
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
        public ActionResult<RoutingData> RouteByServiceType(string serviceType)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            if (ip is null)
                return UnprocessableEntity("No Remote Ip Address");

            var port = HttpContext.Connection.RemotePort.ToString();

            if(_subscriptionUC.CheckIfServiceIsSubscribed(ip, port))
                return Unauthorized("Subscribe your service before making routing requests");

            return Try.WithConsequence(() =>
            {
                var address = _routingUC.RouteByDestinationType(serviceType);

                var routingData = new RoutingData() { Address = address };

                _headersUc.AddJsonHeader(routingData);

                _headersUc.AddAuthorizationHeaders(routingData, serviceType);

                return routingData;
            });
        }
    }
}