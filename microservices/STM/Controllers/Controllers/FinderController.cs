using System.Globalization;
using ApplicationLogic.DTO;
using ApplicationLogic.Use_Cases;
using Entities.Common.Concretions;
using Entities.Transit.Concretions;
using Entities.Transit.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]/[action]")]
    public class FinderController : ControllerBase
    {
        private readonly ILogger<FinderController> _logger;
        private readonly Itinerary _itinerary;

        public FinderController(ILogger<FinderController> logger, Itinerary itinerary)
        {
            _logger = logger;
            _itinerary = itinerary;
        }

        /// <remarks>
        /// Find optimal STM bus to travel between two coordinates
        /// </remarks>
        /// <param name="fromLatitudeLongitude">Latitude, longitude of the first stop</param>
        /// <param name="toLatitudeLongitude">Latitude, longitude of the last stop</param>
        /// <returns>
        /// Json containing the ID of the bus, its trip ID, its name, the ETA, the first and last stops of the trip 
        /// </returns>
        [HttpGet] 
        [ActionName(nameof(OptimalBuses))]
        public async Task<ActionResult<IEnumerable<BusDto>>> OptimalBuses(string fromLatitudeLongitude, string toLatitudeLongitude)
        {
            _logger.LogInformation($"OptimalBus endpoint called with coordinated: from: {fromLatitudeLongitude}; to: {toLatitudeLongitude}");

            var (fromLatitude, fromLongitude, toLatitude, toLongitude) = ParseParams();

            var busTuples = await GetBuses();

            if (busTuples is null) return UnprocessableEntity("No real time data on buses were found for the selected coordinates");

            return FormatToDto(busTuples).ToList();

            async Task<(IBus bus, double eta)[]?> GetBuses()
            {
                var busesAndEtas = await _itinerary.GetFastestBus(
                    new PositionLL() { Latitude = fromLatitude, Longitude = fromLongitude },
                    new PositionLL() { Latitude = toLatitude, Longitude = toLongitude });

                return busesAndEtas;
            }

            IEnumerable<BusDto> FormatToDto((IBus bus, double eta)[] valueTuples)
            {
                foreach (var tuple in valueTuples)
                {
                    var relevantOrigin = tuple.bus!.TransitTrip.RelevantOrigin!.Value;
                    var relevantDestination = tuple.bus.TransitTrip.RelevantDestination!.Value;

                    var busDTO = new BusDto()
                    {
                        BusId = tuple.bus.Id,
                        TripId = tuple.bus.TransitTrip.Id,
                        Name = tuple.bus.Name,
                        ETA = tuple.eta.ToString(),
                        StopIndexAtTimeOfProcessing = tuple.bus.StopIndexAtComputationTime,
                        OriginStopSchedule = new StopScheduleDto()
                        {
                            Index = relevantOrigin.Index,
                            DepartureTime = relevantOrigin.DepartureTime.ToString(CultureInfo.InvariantCulture),
                            Stop = new StopDto()
                            {
                                Message = ((Stop)relevantOrigin.Stop).Message, Id = relevantOrigin.Stop.Id,
                                Position = new PositionDto()
                                {
                                    Latitude = relevantOrigin.Stop.Position.Latitude.ToString(CultureInfo.InvariantCulture),
                                    Longitude = relevantOrigin.Stop.Position.Longitude.ToString(CultureInfo.InvariantCulture),
                                }
                            }
                        },
                        TargetStopSchedule = new StopScheduleDto()
                        {
                            Index = relevantDestination.Index,
                            DepartureTime = relevantDestination.DepartureTime.ToString(CultureInfo.InvariantCulture),
                            Stop = new StopDto()
                            {
                                Message = ((Stop)relevantDestination.Stop).Message, Id = relevantDestination.Stop.Id,
                                Position = new PositionDto()
                                {
                                    Latitude = relevantDestination.Stop.Position.Latitude.ToString(CultureInfo.InvariantCulture),
                                    Longitude = relevantDestination.Stop.Position.Longitude.ToString(CultureInfo.InvariantCulture),
                                }
                            }
                        },
                    };

                    yield return busDTO;
                }
            }

            (double fromLatitude, double fromLongitude, double toLatitude, double toLongitude) ParseParams()
            {
                var fromPositionStrings = fromLatitudeLongitude.Split(',');
                var toPositionStrings = toLatitudeLongitude.Split(',');

                double.TryParse(fromPositionStrings[0].Trim(), out var formattedFromLat);
                double.TryParse(fromPositionStrings[1].Trim(), out var formattedFromLon);
                double.TryParse(toPositionStrings[0].Trim(), out var formattedToLat);
                double.TryParse(toPositionStrings[1].Trim(), out var formattedToLon);

                return (formattedFromLat, formattedFromLon, formattedToLat, formattedToLon);
            }
        }
    }
}