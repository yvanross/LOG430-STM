using ApplicationLogic.Usecases;
using Ingress.Dto;
using Ingress.Repository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Ingress.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly SubscriptionUC _subscriptionUc;

        private readonly MonitorUc _monitorUc;

        private readonly ILogger<SubscriptionController> _logger;

        public SubscriptionController(ILogger<SubscriptionController> logger)
        {
            _logger = logger;

            var _writeModel = new RepositoryWrite();

            _subscriptionUc = new SubscriptionUC(_writeModel);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Post([FromRoute] string id, [FromBody] CallingServiceInfo callingServiceInfo)
        {
            try
            {
                _logger.LogInformation($"{id} attempting to subscribe");

                if (string.IsNullOrEmpty(callingServiceInfo.Address) || string.IsNullOrEmpty(callingServiceInfo.Port)) throw new Exception("Source couldn't be determined");

                _subscriptionUc.Subscribe(id, callingServiceInfo.Address, callingServiceInfo.Port);

                _logger.LogInformation($"{id} from {callingServiceInfo.Address}:{callingServiceInfo.Port} has subscribed");
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