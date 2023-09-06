using Domain.Aggregates.Trip;
using Domain.Factories;

namespace Domain.Services.Aggregates;

public class TripServices
{
    public Trip CreateTrip(string tripId, IEnumerable<(string stopId, TimeSpan schedule)> stopSchedules)
    {
        return TripFactory.Create(tripId, stopSchedules);
    }
}