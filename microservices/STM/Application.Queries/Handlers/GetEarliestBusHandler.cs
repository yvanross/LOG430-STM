using Application.Queries.Seedwork;
using Application.QueryServices;
using Application.ViewModels;

namespace Application.Queries.Handlers;

public class GetEarliestBusHandler : IQueryHandler<GetEarliestBus, RideViewModel>
{
    private readonly ApplicationStopService _stopServices;
    private readonly ApplicationTripServices _tripServices;
    private readonly ApplicationBusServices _busServices;

    public GetEarliestBusHandler(
        ApplicationStopService stopServices,
        ApplicationTripServices tripServices,
        ApplicationBusServices busServices)
    {
        _stopServices = stopServices;
        _tripServices = tripServices;
        _busServices = busServices;
    }

    public async Task<RideViewModel> Handle(GetEarliestBus query, CancellationToken cancellation)
    {
        var sourceStops = await _stopServices.GetClosestStops(query.From);

        var destinationStops = await _stopServices.GetClosestStops(query.To);

        var trips = _tripServices.TimeRelevantTripsContainingSourceAndDestination(sourceStops, destinationStops);

        var relevantBuses = _busServices.GetTimeRelevantRideViewModels(
            trips.ToDictionary(trip => trip.Id),
            sourceStops.ToDictionary(stop => stop.Id),
            destinationStops.ToDictionary(stop => stop.Id));

        return relevantBuses.First();
    }
}