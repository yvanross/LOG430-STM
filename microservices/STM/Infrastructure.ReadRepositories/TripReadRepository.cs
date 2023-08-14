using Application.ReadServices.ServiceInterfaces.Repositories;
using Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ReadRepositories;

public class TripReadRepository : DbContext, ITripReadRepository
{
    private readonly ILogger<TripReadRepository> _logger;

    public DbSet<Trip> Trips { get; set; } = null!;

    public TripReadRepository(DbContextOptions<TripReadRepository> options, ILogger<TripReadRepository> logger) : base(options)
    {
        _logger = logger;
    }

    public IEnumerable<Trip> GetTripsContainingStopsId(IEnumerable<string> stopIds)
    {
        var materializedIds = stopIds.ToList();

        var trips = Trips.Where(trip => trip.StopSchedules.Any(stop => materializedIds.Contains(stop.StopId)));

        return trips;
    }

    public IEnumerable<Trip> GetAll()
    {
        throw new NotImplementedException();
    }

    public Trip Get(string id)
    {
        throw new NotImplementedException();
    }
}