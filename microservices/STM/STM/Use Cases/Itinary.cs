using System.Collections.Immutable;
using Entities.Domain;
using StaticGTFS;
using STM.ApplicationLogic;
using STM.Controllers;
using STM.Entities.Domain;
using STM.ExternalServiceProvider;

namespace STM.Use_Cases
{
    public class Itinary : ChaosDaemon
    {
        private ExternalSTMGateway _externalStmGateway = new ExternalSTMGateway();

        private ILogger? _logger;

        public Itinary(ILogger<STMOptimalBusController> logger = null) => _logger = logger;

        public async Task<(IBus? bus, double eta)?> GetFastestBus(IPosition from, IPosition to)
        {
            try
            {
                IsChaosEnabled();

                STMData.PrefetchData();

                var relevantTrips = RelevantTripsAndRelevantStops(from, to);

                _logger?.LogInformation($"found {relevantTrips?.Length} relevant trips");

                if ((relevantTrips?.Length ?? 0) < 1) return null;

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

        private ITripSTM[]? RelevantTripsAndRelevantStops(IPosition from, IPosition to)
        {
            ItinaryModel itinaryModel = new ItinaryModel(_logger);

            var stopsOrigin = itinaryModel.GetClosestStops(from, STMData.Stops);
            var stopsTarget = itinaryModel.GetClosestStops(to, STMData.Stops);

            var relevantTrips = itinaryModel.TripsContainingSourceAndDestination(stopsOrigin, stopsTarget, STMData.Trips);

            return relevantTrips;
        }

        private async Task<(IBus bus, double eta)[]> RelevantBuses(ITripSTM[] relevantTrips)
        {
            GTFSModel gtfsModel = new GTFSModel(_logger);

            var feedPositions = await _externalStmGateway.RequestFeedPositions();
            
            var feedTripUpdates = (await _externalStmGateway.RequestFeedTripUpdates())
                .ToImmutableDictionary(t => t.Trip.TripId);

            var relevantBuses = gtfsModel.GetVehiculeOnRelevantTrips(relevantTrips, feedPositions, feedTripUpdates);
            
            relevantBuses = gtfsModel.GetRelevantOriginAndDestinationForRelevantBuses(relevantBuses);

            var timeRelevantBuses = gtfsModel.GetTimeRelevantBuses(relevantBuses).ToArray();
            
            return timeRelevantBuses;
        }
    }
}
