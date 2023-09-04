using Application.Queries.Seedwork;
using Application.QueryServices;
using Application.QueryServices.ServiceInterfaces;
using Application.ViewModels;
using Microsoft.Extensions.Logging;

namespace Application.Queries.Handlers;

public class GetEarliestBusHandler : IQueryHandler<GetEarliestBus, RideViewModel>
{
    private readonly ApplicationBusServices _busServices;
    private readonly ILogger<GetEarliestBusHandler> _logger;
    private readonly ApplicationStopService _stopServices;
    private readonly IApplicationTripService _tripService;

    public GetEarliestBusHandler(
        ApplicationStopService stopServices,
        IApplicationTripService applicationTripService,
        ApplicationBusServices busServices,
        ILogger<GetEarliestBusHandler> logger)
    {
        _stopServices = stopServices;
        _tripService = applicationTripService;
        _busServices = busServices;
        _logger = logger;
    }

    public async Task<RideViewModel> Handle(GetEarliestBus query, CancellationToken cancellation)
    {
        try
        {
            var sourceStops = await _stopServices.GetClosestStops(query.From);

            var destinationStops = await _stopServices.GetClosestStops(query.To);

            var trips = await _tripService.TimeRelevantTripsContainingSourceAndDestination(
                sourceStops,
                destinationStops);

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