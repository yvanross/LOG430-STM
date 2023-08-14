using System.Globalization;
using ApplicationLogic.DTO;
using ApplicationLogic.Interfaces;
using Domain.Aggregates;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Controllers.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]/[action]")]
    public class FinderController : ControllerBase
    {
        private readonly ILogger<FinderController> _logger;
        private readonly IItinerary _itinerary;

        public FinderController(ILogger<FinderController> logger, IItinerary itinerary)
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
                    var relevantOrigin = tuple.bus!.TransitTripId.RelevantOrigin!.Value;
                    var relevantDestination = tuple.bus.TransitTripId.RelevantDestination!.Value;

                    var busDTO = new BusDto()
                    {
                        BusId = tuple.bus.Id,
                        TripId = tuple.bus.TransitTripId.Id,
                        Name = tuple.bus.Name,
                        ETA = tuple.eta.ToString(),
                        StopIndexAtTimeOfProcessing = tuple.bus.StopIndexAtComputationTime,
                        OriginStopSchedule = new StopScheduleDto()
                        {
                            Index = relevantOrigin.Index,
                            DepartureTime = relevantOrigin.DepartureTime.ToString(CultureInfo.InvariantCulture),
                            Stop = new StopDto()
                            {
                                Message = ((Stop)relevantOrigin.StopId).Message, Id = relevantOrigin.StopId.Id,
                                Position = new PositionDto()
                                {
                                    Latitude = relevantOrigin.StopId.Position.Latitude.ToString(CultureInfo.InvariantCulture),
                                    Longitude = relevantOrigin.StopId.Position.Longitude.ToString(CultureInfo.InvariantCulture),
                                }
                            }
                        },
                        TargetStopSchedule = new StopScheduleDto()
                        {
                            Index = relevantDestination.Index,
                            DepartureTime = relevantDestination.DepartureTime.ToString(CultureInfo.InvariantCulture),
                            Stop = new StopDto()
                            {
                                Message = ((Stop)relevantDestination.StopId).Message, Id = relevantDestination.StopId.Id,
                                Position = new PositionDto()
                                {
                                    Latitude = relevantDestination.StopId.Position.Latitude.ToString(CultureInfo.InvariantCulture),
                                    Longitude = relevantDestination.StopId.Position.Longitude.ToString(CultureInfo.InvariantCulture),
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