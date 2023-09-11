using Domain.Aggregates.Trip;

namespace Domain.Factories;

internal static class TripFactory
{
    internal static Trip Create(string id, List<(string id, string stopId, TimeSpan departureTimeSpan)> stopSchedules)
    {
        var scheduledStops = stopSchedules
            .Select(x => new ScheduledStop(x.id, x.stopId, x.departureTimeSpan))
            .ToList();

        return new Trip(id, scheduledStops);
    }

    internal static Trip Create(string id, IEnumerable<(string stopId, TimeSpan schedule)> stopSchedules)
    {
        var scheduledStops = stopSchedules
            .Select(x => new ScheduledStop(Guid.NewGuid().ToString(), x.stopId, x.schedule))
            .ToList();

        return new Trip(id, scheduledStops);
    }
}