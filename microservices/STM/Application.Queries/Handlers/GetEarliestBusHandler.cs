using Application.Common.AntiCorruption;
using Application.ReadServices;
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

    public Task<RideViewModel> Handle(GetEarliestBus query)
    {
        var sourceStops = _stopServices.GetClosestStops(query.from);

        var destinationStops = _stopServices.GetClosestStops(query.to);

        var trips = _tripServices.TimeRelevantTripsContainingSourceAndDestination(sourceStops, destinationStops);

        var relevantBuses = _busServices.GetTimeRelevantRideViewModels(
            trips.ToDictionary(trip => trip.Id),
            sourceStops.ToDictionary(stop => stop.Id),
            destinationStops.ToDictionary(stop => stop.Id));

        return Task.FromResult(relevantBuses.First());
    }
}