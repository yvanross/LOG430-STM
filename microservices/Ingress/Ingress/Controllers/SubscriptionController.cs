using ApplicationLogic.Usecases;
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
        public async Task<IActionResult> Post([FromRoute] string id, [FromBody] dynamic body)
        {
            try
            {
                _logger.LogInformation($"{id} attempting to subscribe");

                if (string.IsNullOrEmpty(body.address) || string.IsNullOrEmpty(body.port)) throw new Exception("Source couldn't be determined");

                _subscriptionUc.Subscribe(id, body.address, body.port);

                _logger.LogInformation($"{id} from {body.address}:{body.port} has subscribed");
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