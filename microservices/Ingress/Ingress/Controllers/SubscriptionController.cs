using ApplicationLogic.Dto;
using ApplicationLogic.Usecases;
using Microsoft.AspNetCore.Mvc;

namespace Ingress.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ILogger<SubscriptionController> _logger;
        private readonly Subscription _subscription;

        public SubscriptionController(ILogger<SubscriptionController> logger, Subscription subscription)
        {
            _logger = logger;
            _subscription = subscription;
        }

        [HttpPost("{teamName}/{username}")]
        public async Task<IActionResult> Post([FromRoute] string teamName, [FromRoute] string username, [FromBody] CallingServiceInfo callingServiceInfo)
        {
            try
            {
                _logger.LogInformation($"{username} from {teamName} attempting to subscribe");

                if (string.IsNullOrEmpty(callingServiceInfo.Address) || string.IsNullOrEmpty(callingServiceInfo.Port)) throw new Exception("Source couldn't be determined");

                await _subscription.Subscribe(teamName, username, callingServiceInfo.Address, callingServiceInfo.Port, callingServiceInfo.Secret, callingServiceInfo.Version);

                _logger.LogInformation($"{teamName} from {callingServiceInfo.Address}:{callingServiceInfo.Port} has subscribed");
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