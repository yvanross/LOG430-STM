using Domain.Aggregates.Stop;
using Domain.Aggregates.Trip;
using Domain.Common.Interfaces;
using Domain.Factories;

namespace Domain.Services.Aggregates;

public class TripServices
{
    public Trip CreateTrip(string tripId, IEnumerable<(string stopId, DateTime schedule)> stopSchedules)
    {
        return TripFactory.Create(tripId, stopSchedules);
    }
}