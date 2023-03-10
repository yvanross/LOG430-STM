using Ambassador.BusinessObjects;
using Ambassador.Dto;
using ApplicationLogic.Services;
using ApplicationLogic.Usecases;
using Elfie.Serialization;
using Ingress.Extensions;
using Ingress.Repository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Monitor.Docker;

namespace Ingress.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]/{source}/[action]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly SubscriptionUC _subscriptionUc;

        private readonly MonitorUc _monitorUc;

        private readonly ILogger<SubscriptionController> _logger;

        public SubscriptionController(ILogger<SubscriptionController> logger, string source)
        {
            _logger = logger;

            _monitorUc = new(new LocalDockerClient(), new HeartBeatService(new RepositoryRead(source), new RepositoryWrite()));
        }

        [HttpPut]
        [ActionName(nameof(Subscribe))]
        public async Task<IActionResult> Subscribe(SubscriptionDto subscriptionDto)
        {
            try
            {
                _logger.LogInformation($"{subscriptionDto.ServiceType} attempting to subscribe");

                var container = await _monitorUc.GetPort(subscriptionDto.ContainerId);

                if (string.IsNullOrEmpty(container.Port)) throw new Exception("Source port couldn't be determined");

                _logger.LogInformation($"Calling port: {container.Port}");

                _subscriptionUc.Subscribe(subscriptionDto, container);

                _logger.LogInformation($"{subscriptionDto.ServiceType} from {subscriptionDto.ServiceAddress}:{container.Port} has subscribed");
            }
            catch (Exception e)
            {
                var errorMessage = $"{e.Message} \n \t{e.StackTrace}";

                _logger.LogError(errorMessage);

                return Problem(errorMessage);
            }

            return Ok();
        }
    }
}