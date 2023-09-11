using Application.Queries.Seedwork;
using Application.QueryServices;
using Application.ViewModels;
using Microsoft.Extensions.Logging;

namespace Application.Queries.GetEarliestBus;

public class GetEarliestBusHandler : IQueryHandler<GetEarliestBusQuery, RideViewModel>
{
    private readonly ApplicationBusServices _busServices;
    private readonly ILogger<GetEarliestBusHandler> _logger;
    private readonly ApplicationStopService _stopServices;
    private readonly ApplicationTripService _tripService;

    public GetEarliestBusHandler(
        ApplicationStopService stopServices,
        ApplicationTripService applicationTripService,
        ApplicationBusServices busServices,
        ILogger<GetEarliestBusHandler> logger)
    {
        _stopServices = stopServices;
        _tripService = applicationTripService;
        _busServices = busServices;
        _logger = logger;
    }

    public async Task<RideViewModel> Handle(GetEarliestBusQuery query, CancellationToken cancellation)
    {
        try
        {
            var sourceStops = await _stopServices.GetClosestStops(query.From);

            var destinationStops = await _stopServices.GetClosestStops(query.To);

            var tripProjection = await _tripService.TimeRelevantTripsContainingSourceAndDestination(
                sourceStops,
                destinationStops);

            var relevantBuses = _busServices.GetTimeRelevantRideViewModels(
                tripProjection,
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