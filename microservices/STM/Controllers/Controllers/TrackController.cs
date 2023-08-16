using System.Collections.Concurrent;
using ApplicationLogic.Interfaces;
using ApplicationLogic.Use_Cases;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]/[action]")]
    public class TrackController : ControllerBase
    {
        private readonly ILogger<TrackController> _logger;
        private readonly ITrackBus _trackBus;

        private static readonly ConcurrentDictionary<string, ITrackBus> BusesBeingTracked = new ();

        public TrackController(ILogger<TrackController> logger, ITrackBus trackBus)
        {
            _logger = logger;
            _trackBus = trackBus;
        }

        /// <remarks>
        /// Allows the real-time tracking of a bus. Call-Back will be issued once the bus reached its target destination
        /// </remarks>
        /// <param name="busID">TripId of the bus to track</param>
        /// <param name="tripID">TripId of the trip taken by the bus to track</param>
        /// <param name="originStopID">ID of the stop at which to begin tracking the bus</param>
        /// <param name="targetStopID">ID of the stop at which to stop tracking the bus</param>
        /// <param name="callback">Address to call once the tracking is complete</param>
        /// <returns>Number of real time seconds the bus took to travel from origin to target destinations</returns>
        [HttpPost]
        [ActionName(nameof(BeginTracking))]
        public IActionResult BeginTracking(TrackBus)
        {
            _logger.LogInformation("TrackBus endpoint reached");


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

