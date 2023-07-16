using ApplicationLogic.Interfaces;
using ApplicationLogic.Services.Itinerary;
using Entities.Common.Interfaces;
using Entities.Transit.Interfaces;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Use_Cases
{
    public class Itinerary : IItinerary
    {
        private readonly IStmClient _client;
        private readonly ITransitDataCache _stmData;
        private readonly ILogger _logger;
        private readonly ItineraryPlannerService _itineraryPlannerService;
        private readonly GtfsProcessorService _gtfsProcessorService;

        public Itinerary(
            IStmClient client,
            ITransitDataCache igtfsAggregateRoot,
            ILogger<IItinerary> logger,
            ItineraryPlannerService itineraryPlannerService,
            GtfsProcessorService gtfsProcessorService)
        {
            _client = client;
            _stmData = igtfsAggregateRoot;
            _logger = logger;
            _itineraryPlannerService = itineraryPlannerService;
            _gtfsProcessorService = gtfsProcessorService;
        }

        public async Task<(IBus bus, double eta)[]?> GetFastestBus(IPosition from, IPosition to)
        {
            await PrefetchAndApplyTripUpdates();

            var relevantTrips = RelevantTripsAndRelevantStops(from, to);

            _logger.LogInformation($"found {relevantTrips?.Count} relevant trips");

            if ((relevantTrips?.Count ?? 0) < 1) return default;

            var timeRelevantBuses = RelevantBuses(relevantTrips);

            _logger?.LogInformation($"found {timeRelevantBuses.Length} relevant buses");

            if (!timeRelevantBuses.Any()) return default;

            var optimalBus = timeRelevantBuses.ToList().OrderBy(eta => eta.eta);

            return optimalBus.ToArray();
        }

        public async Task PrefetchAndApplyTripUpdates()
        {
            await _stmData.PrefetchData();

            _logger.LogInformation("# Cache Ready");

            var feedTripUpdates = (await _client.RequestFeedTripUpdates()).ToList();

            _logger.LogInformation("# Trip Updates Fetched");

            _stmData.ApplyTripUpdatesToDataSet(feedTripUpdates);

            _logger.LogInformation("# Trip Updates Applied to cache");
        }

        private Dictionary<string, ITransitTrip> RelevantTripsAndRelevantStops(IPosition from, IPosition to)
        {
            var stops = _stmData.GetStops()!.Values.ToArray();

            var stopsOrigin = _itineraryPlannerService.GetClosestStops(from, stops);
            var stopsTarget = _itineraryPlannerService.GetClosestStops(to, stops);

            var relevantTrips = _itineraryPlannerService.TripsContainingSourceAndDestination(stopsOrigin, stopsTarget);

            return relevantTrips;
        }

        private (IBus bus, double eta)[] RelevantBuses(Dictionary<string, ITransitTrip>? relevantTrips)
        {
            var feedPositions = _client.RequestFeedPositions();

            var relevantBuses = _gtfsProcessorService.GetVehicleOnRelevantTrips(relevantTrips, feedPositions);
            
            relevantBuses = _gtfsProcessorService.GetRelevantOriginAndDestinationForRelevantBuses(relevantBuses);

            var timeRelevantBuses = _gtfsProcessorService.GetTimeRelevantBuses(relevantBuses).ToArray();
            
            return timeRelevantBuses;
        }
    }
}
