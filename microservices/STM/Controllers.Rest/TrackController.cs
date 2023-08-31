using Application.Commands;
using Application.Commands.Seedwork;
using Application.EventHandlers.AntiCorruption;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Controllers.Rest
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TrackController : ControllerBase
    {
        private readonly ILogger<TrackController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IConsumer _consumer;

        public TrackController(ILogger<TrackController> logger, ICommandDispatcher commandDispatcher, IConsumer consumer)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
            _consumer = consumer;
        }

        [HttpPost]
        [ActionName(nameof(BeginTracking))]
        public IActionResult BeginTracking([FromBody] TrackBus trackBus)
        {
            _logger.LogInformation("TrackBus endpoint reached");

            _commandDispatcher.DispatchAsync(trackBus, CancellationToken.None);

            return Accepted();
        }

        [HttpGet]
        [ActionName(nameof(GetTrackingUpdate))]
        public async Task<ActionResult<RideTrackingUpdated>> GetTrackingUpdate()
        {
            var update = await _consumer.ConsumeNext<RideTrackingUpdated>();

            if (update is null)
            {
                return NoContent();
            }

            return Ok(update);
        }
    }
}

