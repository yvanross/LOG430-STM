using System.Collections.Immutable;
using Application.ReadServices.Seedwork;
using Domain.Aggregates;
using Domain.Services;
using Domain.ValueObjects;

namespace Application.ReadServices;

public class ApplicationStopService
{
    private readonly IReadRepository<Stop> _readStops;
    private readonly IStopDomainServices _stopDomainServices;

    public ApplicationStopService(IReadRepository<Stop> readStops, IStopDomainServices stopDomainServices)
    {
        _readStops = readStops;
        _stopDomainServices = stopDomainServices;
    }

    public ImmutableHashSet<Stop> GetClosestStops(Position position)
    {
        const int radiusForBusStopSelectionInMeters = 50;

        var stops = _readStops.GetAll();

        var closestStop = _stopDomainServices.FindClosestStop(position, stops);

        var closestStops = _stopDomainServices.GetNearbyStops(closestStop.Position, stops, radiusForBusStopSelectionInMeters);

        return closestStops.ToImmutableHashSet();
    }
}