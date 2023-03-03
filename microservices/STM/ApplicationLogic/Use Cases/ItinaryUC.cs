using System.Collections.Immutable;
using ApplicationLogic.Interfaces;
using ApplicationLogic.Services;
using Entities.Concretions;
using Entities.Domain;
using GTFS;
using GTFS.Concretions;
using GTFS.Interfaces;
using Microsoft.Extensions.Logging;
using STM.ExternalServiceProvider.Proto;

namespace ApplicationLogic.Use_Cases
{
    public class ItinaryUC
    {
        private readonly IStmClient _client;

        private readonly ILogger? _logger;

        public ItinaryUC(IStmClient client, ILogger? logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<(IBus? bus, double eta)?> GetFastestBus(IPosition from, IPosition to)
        {
            try
            {
                STMData.PrefetchData();

                var feedTripUpdates = (await _client.RequestFeedTripUpdates()).ToList();

                var realtimeTrips = STMData.AddRealtimeGTFSData(feedTripUpdates);

                var relevantTrips = RelevantTripsAndRelevantStops(from, to, realtimeTrips);

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

        private ITripSTM[]? RelevantTripsAndRelevantStops(IPosition from, IPosition to, List<IGTFSTrip> tripsCopy)
        {
            var itinaryService = new ItinaryService(_logger);

            var stops = STMData.Stops!.Values.ToArray();

            var stopsOrigin = itinaryService.GetClosestStops(from, stops);
            var stopsTarget = itinaryService.GetClosestStops(to, stops);

            var relevantTrips = itinaryService.TripsContainingSourceAndDestination(stopsOrigin, stopsTarget, tripsCopy);

            return relevantTrips;
        }

        private async Task<(IBus bus, double eta)[]> RelevantBuses(ITripSTM[] relevantTrips, List<TripUpdate> feedTripUpdates)
        {
            GTFSService gtfsService = new GTFSService();

            var feedPositions = await _client.RequestFeedPositions();

            var relevantBuses = gtfsService.GetVehiculeOnRelevantTrips(relevantTrips, feedPositions, feedTripUpdates.ToImmutableDictionary(x=>x.Trip.TripId));
            
            relevantBuses = gtfsService.GetRelevantOriginAndDestinationForRelevantBuses(relevantBuses);

            var timeRelevantBuses = gtfsService.GetTimeRelevantBuses(relevantBuses).ToArray();
            
            return timeRelevantBuses;
        }
    }
}
