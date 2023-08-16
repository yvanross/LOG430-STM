using Application.QueryServices;
using Application.ViewModels;
using ApplicationLogic.Interfaces;
using ApplicationLogic.Services.Itinerary;
using Domain.Aggregates.Ride;
using Domain.ValueObjects;
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
        private readonly ApplicationStopService _stopServices;
        private readonly ApplicationTripServices _tripServices;
        private readonly ApplicationBusServices _busServices;

        public Itinerary(
            IStmClient client,
            ITransitDataCache igtfsAggregateRoot,
            ILogger<IItinerary> logger,
            ItineraryPlannerService itineraryPlannerService,
            GtfsProcessorService gtfsProcessorService,
            ApplicationStopService stopServices,
            ApplicationTripServices tripServices,
            ApplicationBusServices busServices)
        {
            _client = client;
            _stmData = igtfsAggregateRoot;
            _logger = logger;
            _itineraryPlannerService = itineraryPlannerService;
            _gtfsProcessorService = gtfsProcessorService;
            _stopServices = stopServices;
            _tripServices = tripServices;
            _busServices = busServices;
        }

        public async Task<(Ride bus, double eta)[]?> GetFastestBus(Position from, Position to)
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

        private RideViewModel RelevantBuses(Position from, Position to)
        {
            var sourceStops = _stopServices.GetClosestStops(from);

            var destinationStops = _stopServices.GetClosestStops(to);

            var trips = _tripServices.TimeRelevantTripsContainingSourceAndDestination(sourceStops, destinationStops);

            var relevantBuses = _busServices.GetTimeRelevantRideViewModels(
                trips.ToDictionary(trip => trip.Id), 
                sourceStops.ToDictionary(stop => stop.Id), 
                destinationStops.ToDictionary(stop => stop.Id));
            
            return relevantBuses.First();
        }

        private class TripStopDto
        {

        }
    }
}
