using System.Collections.Immutable;
using Application.QueryServices.Seedwork;
using Domain.Aggregates;
using Domain.Services.Aggregates;
using Domain.ValueObjects;

namespace Application.QueryServices;

public class ApplicationStopService
{
    private readonly IReadRepository<Stop> _readStops;
    private readonly StopServices _stopServices;

    public ApplicationStopService(IReadRepository<Stop> readStops, StopServices stopServices)
    {
        _readStops = readStops;
        _stopServices = stopServices;
    }

    public ImmutableHashSet<Stop> GetClosestStops(Position position)
    {
        const int radiusForBusStopSelectionInMeters = 50;

        var stops = _readStops.GetAllAsync();

        var closestStop = _stopServices.FindClosestStop(position, stops);

        var closestStops = _stopServices.GetNearbyStops(closestStop.Position, stops, radiusForBusStopSelectionInMeters);

        return closestStops.ToImmutableHashSet();
    }
}