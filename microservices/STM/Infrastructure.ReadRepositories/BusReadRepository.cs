using Application.QueryServices.ServiceInterfaces.Repositories;
using Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ReadRepositories;

public class BusReadRepository : ReadRepository<Bus>, IBusReadRepository
{
    public BusReadRepository(DbContextOptions options, ILogger logger) : base(options, logger)
    {
    }

    public IEnumerable<Bus> GetAllIdsMatchingTripsIds(IEnumerable<string> tripIds)
    {
        var materializedTrips = tripIds.ToList();

        var buses = Aggregates.Where(bus => materializedTrips.Contains(bus.TripId));

        return buses.ToList();
    }
}