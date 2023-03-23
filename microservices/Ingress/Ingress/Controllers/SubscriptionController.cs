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
    [Route("[controller]/[action]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly SubscriptionUC _subscriptionUc;

        private readonly MonitorUc _monitorUc;

        private readonly ILogger<SubscriptionController> _logger;

        public SubscriptionController(ILogger<SubscriptionController> logger)
        {
            _logger = logger;

            var _readModel = new RepositoryRead();
            var _writeModel = new RepositoryWrite();
            var _environmentClient = new LocalDockerClient(logger);

            _subscriptionUc = new SubscriptionUC(_writeModel, _readModel, _environmentClient);
            _monitorUc = new(_environmentClient, _readModel, _writeModel);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Post([FromRoute] string id, [FromBody] string address, [FromBody] string port)
        {
            try
            {
                _logger.LogInformation($"{id} attempting to subscribe");

                if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(port)) throw new Exception("Source couldn't be determined");

                await _subscriptionUc.Subscribe(id, address, port);

                _monitorUc.TryScheduleHeartBeatOnScheduler();

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