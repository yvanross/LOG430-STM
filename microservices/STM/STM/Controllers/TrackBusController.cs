using ApplicationLogic.Use_Cases;
using Entities.Concretions;
using Microsoft.AspNetCore.Mvc;
using STM.DTO;
using STM.External;

namespace STM.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TrackBusController : ControllerBase
    {
        private readonly ILogger<TrackBusController> _logger;

        private List<TrackBusUC> _busesBeingTracked = new ();

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

            var relevantOrigin = busDTO.OriginStopSchedule;
            var relevantDestination = busDTO.TargetStopSchedule;

            var stmEta = Convert.ToDouble(busDTO.ETA);

            var bus = new Bus()
            {
                Id = busDTO.BusID,
                Name = busDTO.Name,
                Trip = new TripSTM()
                {
                    RelevantOrigin = new StopScheduleSTM()
                    {
                        Index = relevantOrigin.index,
                        DepartureTime = Convert.ToDateTime(relevantOrigin.DepartureTime),
                        Stop = new StopSTM()
                        {
                            Message = relevantOrigin.Stop.Message,
                            Id = relevantOrigin.Stop.ID,
                            Position = new PositionLL()
                            {
                                Latitude = Convert.ToDouble(relevantOrigin.Stop.Position.Latitude),
                                Longitude = Convert.ToDouble(relevantOrigin.Stop.Position.Longitude)
                            }
                        }
                    },
                    RelevantDestination = new StopScheduleSTM()
                    {
                        Index = relevantDestination.index,
                        DepartureTime = Convert.ToDateTime(relevantDestination.DepartureTime),
                        Stop = new StopSTM()
                        {
                            Message = relevantDestination.Stop.Message,
                            Id = relevantDestination.Stop.ID,
                            Position = new PositionLL()
                            {
                                Latitude = Convert.ToDouble(relevantDestination.Stop.Position.Latitude),
                                Longitude = Convert.ToDouble(relevantDestination.Stop.Position.Longitude)
                            }
                        }
                    },
                    Id = busDTO.TripID,
                },
            };
            var track = new TrackBusUC(bus, stmEta, busDTO.callBack is null ? default : new CallBackClient(busDTO.callBack), new StmClient(), _logger);

            _busesBeingTracked.Add(track);

            _ = track.PerdiodicCaller();
        }
    }
}

