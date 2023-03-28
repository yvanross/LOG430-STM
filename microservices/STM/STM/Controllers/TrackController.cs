using System.Collections.Immutable;
using ApplicationLogic.Use_Cases;
using Entities.Concretions;
using Entities.Domain;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using STM.Dto;
using STM.External;

namespace STM.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]/[action]")]
    public class TrackController : ControllerBase
    {
        private readonly ILogger<TrackController> _logger;

        private static ImmutableDictionary<string, TrackBusUC> _busesBeingTracked = ImmutableDictionary<string, TrackBusUC>.Empty;

        public TrackController(ILogger<TrackController> logger)
        {
            _logger = logger;
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
        public IActionResult BeginTracking(BusDto busDto)
        {
            _logger.LogInformation("TrackBus endpoint reached");

            var relevantOrigin = busDto.OriginStopSchedule;
            var relevantDestination = busDto.TargetStopSchedule;

            var stmEta = Convert.ToDouble(busDto.ETA);

            var bus = new Bus()
            {
                Id = busDto.BusId,
                Name = busDto.Name,
                StopIndexAtComputationTime = busDto.StopIndexAtTimeOfProcessing,
                Trip = new TripSTM()
                {
                    Id = busDto.TripId,
                    RelevantOrigin = new StopScheduleSTM()
                    {
                        Index = relevantOrigin.Index,
                        DepartureTime = Convert.ToDateTime(relevantOrigin.DepartureTime),
                        Stop = new StopSTM()
                        {
                            Message = relevantOrigin.Stop.Message,
                            Id = relevantOrigin.Stop.Id,
                            Position = new PositionLL()
                            {
                                Latitude = Convert.ToDouble(relevantOrigin.Stop.Position.Latitude),
                                Longitude = Convert.ToDouble(relevantOrigin.Stop.Position.Longitude)
                            }
                        }
                    },
                    RelevantDestination = new StopScheduleSTM()
                    {
                        Index = relevantDestination.Index,
                        DepartureTime = Convert.ToDateTime(relevantDestination.DepartureTime),
                        Stop = new StopSTM()
                        {
                            Message = relevantDestination.Stop.Message,
                            Id = relevantDestination.Stop.Id,
                            Position = new PositionLL()
                            {
                                Latitude = Convert.ToDouble(relevantDestination.Stop.Position.Latitude),
                                Longitude = Convert.ToDouble(relevantDestination.Stop.Position.Longitude)
                            }
                        }
                    },
                },
            };

            var track = new TrackBusUC(bus, stmEta, new StmClient(), _logger);

            _busesBeingTracked = _busesBeingTracked.Add(bus.Id, track);

            return Accepted();
        }

        [HttpGet]
        [ActionName(nameof(GetTrackingUpdate))]
        public ActionResult<IBusTracking> GetTrackingUpdate([FromQuery] string busId)
        {
            if (_busesBeingTracked.ContainsKey(busId))
            {
                var update = _busesBeingTracked[busId].GetUpdate();

                if (update is null)
                {
                    _busesBeingTracked.Remove(busId);

                    return NoContent();
                }

                return new ActionResult<IBusTracking>(update);
            }

            return NoContent();
        }
    }
}

