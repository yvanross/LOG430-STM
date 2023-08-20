using Application.QueryServices.ServiceInterfaces.Repositories;
using Domain.Aggregates.Trip;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.ReadRepositories;

public class TripReadRepository : ReadRepository<Trip>, ITripReadRepository
{
    public TripReadRepository(AppReadDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Trip>> GetTripsContainingStopsId(IEnumerable<string> stopIds)
    {
        var materializedIds = stopIds.ToList();

        var trips = Aggregates.Where(trip => trip.ScheduledStops.Any(stop => materializedIds.Contains(stop.StopId)));

        var completeTrips = await trips.Include(t => t.ScheduledStops).ToListAsync();

        return completeTrips;
    }

    public override async Task<IEnumerable<Trip>> GetAllAsync()
    {
        return await Aggregates.AsQueryable().Include(a => a.ScheduledStops).ToListAsync();
    }

    public override Task<Trip> GetAsync(string id)
    {
        return Task.FromResult(Aggregates.AsQueryable().Include(x => x.ScheduledStops).First(a => a.Id.SequenceEqual(id)) ??
                               throw new KeyNotFoundException($"Aggregate of type {typeof(Trip)} could not be found using id: {id}"));
    }
}