using System.Collections.Immutable;
using Entities.Concretions;
using Entities.Domain;
using StaticGTFS;
using StaticGTFS.Concretions;
using STM.ApplicationLogic;
using STM.Controllers;
using STM.Entities.Concretions;
using STM.Entities.Domain;
using STM.ExternalServiceProvider;
using STM.ExternalServiceProvider.Proto;
using Position = Entities.Concretions.Position;

namespace STM.Use_Cases
{
    public class Itinary : ChaosDaemon
    {
        private StmClient _stmClient = new StmClient();

        private ILogger? _logger;

        public Itinary(ILogger<STMOptimalBusController> logger = null) => _logger = logger;

        public async Task<(IBus? bus, double eta)?> GetFastestBus(IPosition from, IPosition to)
        {
            try
            {
                IsChaosEnabled();

                STMData.PrefetchData();

                var feedTripUpdates = (await _stmClient.RequestFeedTripUpdates()).ToList();

                //todo actually update trip and not just add
                feedTripUpdates.ForEach(x => STMData.AddTrip(new Trip()
                {
                    Id = x.Trip.TripId,
                    StopSchedules = x.StopTimeUpdate.ToList().ConvertAll(stopTimeU =>
                    {
                        if (STMData.Stops!.ContainsKey(stopTimeU.StopId) is false)
                            STMData.AddStop(new Stop() { ID = stopTimeU.StopId, Position = new Position() });

                        var schedule = (IStopSchedule)new StopSchedule()
                        {
                            Stop = STMData.Stops[stopTimeU.StopId],
                            DepartureTime = (DateTime.UnixEpoch.AddSeconds(stopTimeU.Departure?.Time ?? stopTimeU.Arrival?.Time ?? 0L))
                        };

                        return schedule;
                    }),
                    FromStaticGtfs = false
                }));

                var relevantTrips = RelevantTripsAndRelevantStops(from, to);

                _logger?.LogInformation($"found {relevantTrips?.Length} relevant trips");

                if ((relevantTrips?.Length ?? 0) < 1) return null;

                var timeRelevantBuses = await RelevantBuses(relevantTrips, feedTripUpdates);

                _logger?.LogInformation($"found {timeRelevantBuses.Length} relevant buses");

                if (!timeRelevantBuses.Any()) return null;

                var optimalBus = timeRelevantBuses?.MinBy(eta => eta.eta);

                return new(optimalBus?.bus, optimalBus?.eta ?? double.MaxValue);
            }
            catch (Exception e)
            {
                _logger?.LogCritical(e.Message);
                throw;
            }
        }

        private ITripSTM[]? RelevantTripsAndRelevantStops(IPosition from, IPosition to)
        {
            ItinaryModel itinaryModel = new ItinaryModel(_logger);

            var stops = STMData.Stops!.Values.ToArray();

            var stopsOrigin = itinaryModel.GetClosestStops(from, stops);
            var stopsTarget = itinaryModel.GetClosestStops(to, stops);

            var relevantTrips = itinaryModel.TripsContainingSourceAndDestination(stopsOrigin, stopsTarget, STMData.Trips);

            return relevantTrips;
        }

        private async Task<(IBus bus, double eta)[]> RelevantBuses(ITripSTM[] relevantTrips, List<TripUpdate> feedTripUpdates)
        {
            GTFSModel gtfsModel = new GTFSModel();

            var feedPositions = await _stmClient.RequestFeedPositions();

            var relevantBuses = gtfsModel.GetVehiculeOnRelevantTrips(relevantTrips, feedPositions, feedTripUpdates.ToImmutableDictionary(x=>x.Trip.TripId));
            
            relevantBuses = gtfsModel.GetRelevantOriginAndDestinationForRelevantBuses(relevantBuses);

            var timeRelevantBuses = gtfsModel.GetTimeRelevantBuses(relevantBuses).ToArray();
            
            return timeRelevantBuses;
        }
    }
}
