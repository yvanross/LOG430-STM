using Application.QueryServices.ServiceInterfaces.Repositories;
using Domain.Aggregates;

namespace Infrastructure.ReadRepositories;

public class BusReadRepository : ReadRepository<Bus>, IBusReadRepository
{
    public BusReadRepository(AppReadDbContext context) : base(context)
    {
    }

    public IEnumerable<Bus> GetAllIdsMatchingTripsIds(IEnumerable<string> tripIds)
    {
        var materializedTrips = tripIds.ToList();

        var buses = Aggregates.Where(bus => materializedTrips.Contains(bus.TripId));

        return buses.ToList();
    }
}