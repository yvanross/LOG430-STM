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

        [HttpPost("{group}/{teamName}/{username}")]
        public async Task<IActionResult> Post([FromRoute] string group, [FromRoute] string teamName, [FromRoute] string username, [FromBody] CallingServiceInfo callingServiceInfo)
        {
            try
            {
                _logger.LogInformation($"{username} from {teamName} attempting to subscribe");

                await _subscription.Subscribe(group, teamName, username, callingServiceInfo.Secret, callingServiceInfo.Version);

                _logger.LogInformation($"{username} from {teamName} of group {group} has subscribed");
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