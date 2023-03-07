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

        private readonly IgtfsDataSource _stmData;

        public ItinaryUC(IStmClient client, IgtfsDataSource igtfsDataSource, ILogger? logger)
        {
            _client = client;
            _stmData = igtfsDataSource;
            _logger = logger;
        }

        public async Task<(IBus? bus, double eta)?> GetFastestBus(IPosition from, IPosition to)
        {
            try
            {
                _stmData.PrefetchData();

                var feedTripUpdates = (await _client.RequestFeedTripUpdates()).ToList();

                _stmData.ApplyTripUpdatesToDataSet(feedTripUpdates);

                var relevantTrips = RelevantTripsAndRelevantStops(from, to);

                _logger?.LogInformation($"found {relevantTrips?.Count} relevant trips");

                if ((relevantTrips?.Count ?? 0) < 1) return null;

                var timeRelevantBuses = await RelevantBuses(relevantTrips);

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

        private ImmutableDictionary<string, ITripSTM> RelevantTripsAndRelevantStops(IPosition from, IPosition to)
        {
            var itinaryService = new ItinaryService(_logger, _stmData);

            var stops = _stmData.GetStops()!.Values.ToArray();

            var stopsOrigin = itinaryService.GetClosestStops(from, stops);
            var stopsTarget = itinaryService.GetClosestStops(to, stops);

            var relevantTrips = itinaryService.TripsContainingSourceAndDestination(stopsOrigin, stopsTarget);

            return relevantTrips;
        }

        private async Task<(IBus bus, double eta)[]> RelevantBuses(ImmutableDictionary<string, ITripSTM>? relevantTrips)
        {
            GTFSService gtfsService = new GTFSService();

            var feedPositions = _client.RequestFeedPositions();

            var relevantBuses = gtfsService.GetVehicleOnRelevantTrips(relevantTrips, feedPositions);
            
            relevantBuses = gtfsService.GetRelevantOriginAndDestinationForRelevantBuses(relevantBuses);

            var timeRelevantBuses = gtfsService.GetTimeRelevantBuses(relevantBuses).ToArray();
            
            return timeRelevantBuses;
        }
    }
}
