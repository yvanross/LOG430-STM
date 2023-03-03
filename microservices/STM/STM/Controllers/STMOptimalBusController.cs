using Entities.Concretions;
using Microsoft.AspNetCore.Mvc;
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
    public class STMOptimalBusController : ControllerBase
    {
        private readonly ILogger<STMOptimalBusController> _logger;

        public STMOptimalBusController(ILogger<STMOptimalBusController> logger)
        {
            _logger = logger;
        }

        /// <remarks>
        /// Find optimal STM bus to travel between two coordinates
        /// </remarks>
        /// <param name="fromLatitudeLongitude">Latitude, longitude of the first stop</param>
        /// <param name="toLatitudeLongitude">Latitude, longitude of the last stop</param>
        /// <returns>
        /// Json containing the ID of the bus, its trip ID, its name, the ETA, the first and last stops of the trip 
        /// </returns>
        [HttpGet(Name = "GetBestBus")]
        public async Task<TrackingBusDTO> Get(string fromLatitudeLongitude, string toLatitudeLongitude)
        {
            _logger.LogInformation($"OptimalBus endpoint called with coordinated: from: {fromLatitudeLongitude}; to: {toLatitudeLongitude}");

            Itinary itinary = new Itinary(_logger);

            var fromPositionStrings = fromLatitudeLongitude.Split(',');
            var toPositionStrings = toLatitudeLongitude.Split(',');

            double.TryParse(fromPositionStrings[0].Trim(), out var fromLatitude);
            double.TryParse(fromPositionStrings[1].Trim(), out var fromLongitude);
            double.TryParse(toPositionStrings[0].Trim(), out var toLatitude);
            double.TryParse(toPositionStrings[1].Trim(), out var toLongitude);

            var tuple = await itinary.GetFastestBus(new Position()
            {
                Latitude = fromLatitude,
                Longitude = fromLongitude
            }, new Position()
            {
                Latitude = toLatitude,
                Longitude = toLongitude
            });

            if (tuple == null)
            {
                throw (new Exception($"No buses were found, logging: \n" +
                                     $"time {DateTime.UtcNow} \n" +
                                     $"stm stops {STMData.Stops?.Values.Count() -1} \n" +
                                     $"stm trips {STMData.Trips?.Count ?? -1}"));
            }

            var relevantOrigin = tuple.Value.bus.Trip.RelevantOrigin.Value;
            var relevantDestination = tuple.Value.bus.Trip.RelevantDestination.Value;

            var busDTO = new TrackingBusDTO()
            {
                BusID = tuple.Value.bus.ID,
                TripID = tuple.Value.bus.Trip.ID,
                Name = tuple.Value.bus.Name,
                ETA = tuple.Value.eta.ToString(),
                indexOfOriginStop = tuple.Value.bus.Trip.RelevantOrigin.Value.index,
                indexOfDestinationStop = tuple.Value.bus.Trip.RelevantDestination.Value.index,
                OriginStopSchedule = new StopScheduleDTO()
                {
                    index = relevantOrigin.index,
                    DepartureTime = relevantOrigin.DepartureTime.ToString(),
                    Stop = new StopDTO()
                    {
                        Message = ((StopSTM)relevantOrigin.Stop).Message,
                        ID = relevantOrigin.Stop.ID,
                        Position = new PositionDTO()
                        {
                            Latitude = relevantOrigin.Stop.Position.Latitude.ToString(),
                            Longitude = relevantOrigin.Stop.Position.Longitude.ToString(),
                        }
                    }
                },
                TargetStopSchedule = new StopScheduleDTO()
                {
                    index = relevantDestination.index,
                    DepartureTime = relevantDestination.DepartureTime.ToString(),
                    Stop = new StopDTO()
                    {
                        Message = ((StopSTM)relevantDestination.Stop).Message,
                        ID = relevantDestination.Stop.ID,
                        Position = new PositionDTO()
                        {
                            Latitude = relevantDestination.Stop.Position.Latitude.ToString(),
                            Longitude = relevantDestination.Stop.Position.Longitude.ToString(),
                        }
                    }
                },
            };

            return busDTO;
        }
    }
}