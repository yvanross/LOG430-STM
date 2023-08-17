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

        /// <summary>
        ///   This endpoint is used to begin tracking a bus.
        /// </summary>
        /// <param name="trackBus"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName(nameof(BeginTracking))]
        public IActionResult BeginTracking([FromBody] TrackBus trackBus)
        {
            _logger.LogInformation("TrackBus endpoint reached");

            _commandDispatcher.Dispatch(trackBus, CancellationToken.None);

            return Accepted();
        }

        [HttpGet]
        [ActionName(nameof(GetTrackingUpdate))]
        public async Task<ActionResult<BusTrackingUpdated>> GetTrackingUpdate()
        {
            var update = await _consumer.Consume<BusTrackingUpdated>();

            if (update is null)
            {
                return NoContent();
            }

            return Ok(update);
        }
    }
}

