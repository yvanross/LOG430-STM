using Entities.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using StaticGTFS;
using STM.Entities.Concretions;
using STM.Entities.Domain;
using STM.Entities.DTO;
using STM.ExternalServiceProvider;
using STM.Use_Cases;

namespace STM.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TrackBusController : ControllerBase
    {
        private readonly ILogger<TrackBusController> _logger;

        private List<TrackBus> _busesBeingTracked = new List<TrackBus>();

        public TrackBusController(ILogger<TrackBusController> logger)
        {
            _logger = logger;
        }

        /// <remarks>
        /// Allows the real-time tracking of a bus. Call-Back will be issued once the bus reached its target destination
        /// </remarks>
        /// <param name="busID">TripID of the bus to track</param>
        /// <param name="tripID">TripID of the trip taken by the bus to track</param>
        /// <param name="originStopID">ID of the stop at which to begin tracking the bus</param>
        /// <param name="targetStopID">ID of the stop at which to stop tracking the bus</param>
        /// <param name="callback">Address to call once the tracking is complete</param>
        /// <returns>Number of real time seconds the bus took to travel from origin to target destinations</returns>
        [HttpPost(Name = "Track Bus")]
        public void Post(TrackingBusDTO busDTO)
        {
            _logger.LogInformation("TrackBus endpoint reached");

            var track = new TrackBus(busDTO, _logger);

            _busesBeingTracked.Add(track);

            track.PerdiodicCaller();
        }
    }
}
