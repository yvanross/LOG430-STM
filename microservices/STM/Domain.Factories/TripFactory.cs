using Domain.Aggregates.Trip;

namespace Domain.Factories;

internal static class TripFactory
{
    internal static Trip Create(string id, IEnumerable<(string stopId, DateTime departureTime)> scheduledStops)
    {
        return Trip.CreateTrip(id, scheduledStops);
    }
}