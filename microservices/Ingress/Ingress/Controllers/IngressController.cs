using Ambassador.BusinessObjects;
using ApplicationLogic.Usecases;
using Docker.DotNet;
using Ingress.Extensions;
using Ingress.Repository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Monitor.Docker;

namespace Ingress.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]/[action]")]
    public class IngressController : ControllerBase
    {
        private readonly SubscriptionUC _subscriptionUC = new (new RepositoryWrite(), new RepositoryRead());

        private readonly RoutingUC _routingUC = new (new RepositoryRead());
        
        private readonly MonitorUc _monitorUc = new (new LocalDockerClient());
        
        private readonly HeadersUC _headersUc = new ();

        private readonly ILogger<IngressController> _logger;

        public IngressController(ILogger<IngressController> logger)
        {
            _logger = logger;
        }

        [HttpPut]
        [ActionName(nameof(Subscribe))]
        public async Task<IActionResult> Subscribe(string serviceType, string serviceAddress, string containerId)
        {
            try
            {
                _logger.LogInformation($"{serviceType} attempting to subscribe");

                var port = await _monitorUc.GetPort(containerId);

                if (string.IsNullOrEmpty(port)) throw new Exception("Source port couldn't be determined");

                _logger.LogInformation($"Calling port: {port}");

                _subscriptionUC.Subscribe(serviceAddress, port, serviceType);

                _logger.LogInformation($"{serviceType} from {serviceAddress}:{port} has subscribed");
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
            return Try.WithConsequence(() =>
            {
                var address = _routingUC.RouteByDestinationType(serviceType);

                var routingData = new RoutingData() { Address = address };

                _headersUc.AddJsonHeader(routingData);

                _headersUc.AddAuthorizationHeaders(routingData, serviceType);

                _logger.LogInformation($"routing service to {routingData.Address}");

                return routingData;
            });
        }
    }
}