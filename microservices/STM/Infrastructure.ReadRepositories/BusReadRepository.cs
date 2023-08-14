using Application.ReadServices.ServiceInterfaces.Repositories;
using Domain.Aggregates;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ReadRepositories;

public class BusReadRepository : DbContext, IBusReadRepository
{
    private readonly ILogger<BusReadRepository> _logger;

    public DbSet<Bus> Buses { get; set; } = null!;

    public BusReadRepository(DbContextOptions<BusReadRepository> options, ILogger<BusReadRepository> logger) : base(options)
    {
        _logger = logger;
    }

    public IEnumerable<Bus> GetAll()
    {
        throw new NotImplementedException();
    }

    public Bus Get(string id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Bus> GetAllIdsMatchingTripsIds(IEnumerable<string> tripIds)
    {
        var materializedTrips = tripIds.ToList();

        var buses = Buses.Where(bus => materializedTrips.Contains(bus.TripId));

        return buses.ToList();
    }
}