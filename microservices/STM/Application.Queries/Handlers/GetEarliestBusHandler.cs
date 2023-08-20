using Application.Queries.Seedwork;
using Application.QueryServices;
using Application.ViewModels;
using Microsoft.Extensions.Logging;

namespace Application.Queries.Handlers;

public class GetEarliestBusHandler : IQueryHandler<GetEarliestBus, RideViewModel>
{
    private readonly ApplicationStopService _stopServices;
    private readonly ApplicationTripServices _tripServices;
    private readonly ApplicationBusServices _busServices;
    private readonly ILogger<GetEarliestBusHandler> _logger;

    public GetEarliestBusHandler(
        ApplicationStopService stopServices,
        ApplicationTripServices tripServices,
        ApplicationBusServices busServices,
        ILogger<GetEarliestBusHandler> logger)
    {
        _stopServices = stopServices;
        _tripServices = tripServices;
        _busServices = busServices;
        _logger = logger;
    }

    public async Task<RideViewModel> Handle(GetEarliestBus query, CancellationToken cancellation)
    {
        try
        {
            var sourceStops = await _stopServices.GetClosestStops(query.From);

            var destinationStops = await _stopServices.GetClosestStops(query.To);

            var trips = await _tripServices.TimeRelevantTripsContainingSourceAndDestination(sourceStops, destinationStops);

            var relevantBuses = _busServices.GetTimeRelevantRideViewModels(
                trips.ToDictionary(trip => trip.Id),
                sourceStops.Select(stop => stop.Id).ToHashSet(),
                destinationStops.Select(stop => stop.Id).ToHashSet());

            return relevantBuses.First();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in GetEarliestBusHandler");

            throw;
        }
       
    }
}