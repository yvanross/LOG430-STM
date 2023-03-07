using System.Collections.Generic;
using System.Globalization;
using ApplicationLogic.Extensions;
using ApplicationLogic.Use_Cases;
using Entities.Concretions;
using Entities.Domain;
using GTFS;
using Microsoft.AspNetCore.Mvc;
using STM.DTO;
using STM.External;
using STM.ExternalServiceProvider;
using STM.ExternalServiceProvider.Proto;

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
        public async Task<ActionResult<IEnumerable<TrackingBusDTO>>> Get(string fromLatitudeLongitude, string toLatitudeLongitude)
        {
            _logger.LogInformation($"OptimalBus endpoint called with coordinated: from: {fromLatitudeLongitude}; to: {toLatitudeLongitude}");

            var (fromLatitude, fromLongitude, toLatitude, toLongitude) = ParseParams();

            var busTuples = await Try.WithConsequenceAsync(GetBuses, retryCount: 3);

            if (busTuples is null) return UnprocessableEntity("No real time data on buses were found for the selected coordinates");

            return FormatToDto(busTuples).ToList();

            async Task<(IBus bus, double eta)[]?> GetBuses()
            {
                var stmData = new StmData();

                var itineraryUc = new ItineraryUC(new StmClient(), stmData, _logger);

                var busesAndEtas = await itineraryUc.GetFastestBus(new PositionLL() { Latitude = fromLatitude, Longitude = fromLongitude }, new PositionLL() { Latitude = toLatitude, Longitude = toLongitude });

                return busesAndEtas;
            }

            IEnumerable<TrackingBusDTO> FormatToDto((IBus bus, double eta)[] valueTuples)
            {
                foreach (var tuple in valueTuples)
                {
                    var relevantOrigin = tuple.bus!.Trip.RelevantOrigin!.Value;
                    var relevantDestination = tuple.bus.Trip.RelevantDestination!.Value;

                    var busDTO = new TrackingBusDTO()
                    {
                        BusID = tuple.bus.Id,
                        TripID = tuple.bus.Trip.Id,
                        Name = tuple.bus.Name,
                        ETA = tuple.eta.ToString(),
                        OriginStopSchedule = new StopScheduleDTO()
                        {
                            index = relevantOrigin.Index,
                            DepartureTime = relevantOrigin.DepartureTime.ToString(CultureInfo.InvariantCulture),
                            Stop = new StopDTO()
                            {
                                Message = ((StopSTM)relevantOrigin.Stop).Message, ID = relevantOrigin.Stop.Id,
                                Position = new PositionDTO()
                                {
                                    Latitude = relevantOrigin.Stop.Position.Latitude.ToString(CultureInfo.InvariantCulture),
                                    Longitude = relevantOrigin.Stop.Position.Longitude.ToString(CultureInfo.InvariantCulture),
                                }
                            }
                        },
                        TargetStopSchedule = new StopScheduleDTO()
                        {
                            index = relevantDestination.Index,
                            DepartureTime = relevantDestination.DepartureTime.ToString(CultureInfo.InvariantCulture),
                            Stop = new StopDTO()
                            {
                                Message = ((StopSTM)relevantDestination.Stop).Message, ID = relevantDestination.Stop.Id,
                                Position = new PositionDTO()
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

                double.TryParse(fromPositionStrings[0].Trim(), out var result);
                double.TryParse(fromPositionStrings[1].Trim(), out var d);
                double.TryParse(toPositionStrings[0].Trim(), out var toLatitude1);
                double.TryParse(toPositionStrings[1].Trim(), out var toLongitude1);
                return (result, d, toLatitude1, toLongitude1);
            }
        }
    }
}