using Application.QueryServices.ServiceInterfaces.Repositories;
using Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ReadRepositories;

public class TripReadRepository : ReadRepository<Trip>, ITripReadRepository
{
    public TripReadRepository(DbContextOptions options, ILogger logger) : base(options, logger)
    {
    }

    public IEnumerable<Trip> GetTripsContainingStopsId(IEnumerable<string> stopIds)
    {
        var materializedIds = stopIds.ToList();

        var trips = Aggregates.Where(trip => trip.ScheduledStops.Any(stop => materializedIds.Contains(stop.StopId)));

        return trips;
    }
}