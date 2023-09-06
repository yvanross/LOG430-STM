using Domain.Aggregates.Trip;

namespace Domain.Factories;

internal static class TripFactory
{
    internal static Trip Create(string id, List<(string id, string stopId, TimeSpan departureTimeSpan)> scheduledStops)
    {
        return Trip.CreateTrip(id, scheduledStops);
    }

    public static Trip Create(string id, IEnumerable<(string stopId, TimeSpan schedule)> scheduledStops)
    {
        return Trip.CreateTrip(id, scheduledStops.ToList().ConvertAll(scheduledStop => (Guid.NewGuid().ToString(), scheduledStop.stopId, scheduledStop.schedule)));
    }
}