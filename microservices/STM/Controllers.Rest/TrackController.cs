using Application.Commands;
using Application.Commands.AntiCorruption;
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

        public TrackController(ILogger<TrackController> logger, ICommandDispatcher commandDispatcher)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
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
        public ActionResult<IBusTracking> GetTrackingUpdate([FromQuery] string busId)
        {
            if (BusesBeingTracked.TryGetValue(busId, out var value))
            {
                var update = value.GetUpdate();

                if(update is null)
                    return NoContent();

                if (update.TrackingCompleted)
                {
                    BusesBeingTracked.Remove(busId, out _);

                    return NoContent();
                }

                return Ok(update);
            }

            return NoContent();
        }
    }
}

