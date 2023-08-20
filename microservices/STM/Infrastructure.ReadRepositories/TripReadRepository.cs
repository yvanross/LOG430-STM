using Application.QueryServices.ServiceInterfaces.Repositories;
using Domain.Aggregates;

namespace Infrastructure.ReadRepositories;

public class TripReadRepository : ReadRepository<Trip>, ITripReadRepository
{
    public TripReadRepository(AppReadDbContext context) : base(context)
    {
    }

    public IEnumerable<Trip> GetTripsContainingStopsId(IEnumerable<string> stopIds)
    {
        var materializedIds = stopIds.ToList();

        var trips = Aggregates.Where(trip => trip.ScheduledStops.Any(stop => materializedIds.Contains(stop.StopId)));

        return trips;
    }
}