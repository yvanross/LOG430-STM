using Domain.Aggregates.Stop;
using Domain.Common.Exceptions;
using Domain.ValueObjects;

namespace Domain.Services.Aggregates;

public class StopServices
{
    public IEnumerable<Stop> GetNearbyStops(Position referencePosition, IEnumerable<Stop> stops,
        int radiusForBusStopSelection)
    {
        const int numberOfStopsInBatch = 15;
        const int radiusIncreaseForNextBatch = 50;

        var nearbyStops = (
                from stop in stops
                let temp = referencePosition.DistanceInMeters(stop.Position)
                where radiusForBusStopSelection > temp
                select stop)
            .ToList();

        return nearbyStops.Count > numberOfStopsInBatch
            ? nearbyStops
            : GetNearbyStops(referencePosition, stops, radiusForBusStopSelection + radiusIncreaseForNextBatch);
    }

    public Stop FindClosestStop(Position position, IEnumerable<Stop> stops)
    {
        return stops.MinBy(s => position.DistanceInMeters(s.Position)) ?? throw new StopNotFoundException();
    }
}