using Domain.Aggregates;
using Domain.ValueObjects;

namespace Domain.Services;

public interface IStopDomainServices
{
    IEnumerable<Stop> GetNearbyStops(Position referencePosition, IEnumerable<Stop> stops, int radiusForBusStopSelection);
    Stop FindClosestStop(Position position, IEnumerable<Stop> stops);
}