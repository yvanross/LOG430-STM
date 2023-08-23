using Application.CommandServices.ServiceInterfaces.Repositories;
using Domain.Aggregates.Trip;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.WriteRepositories;

public class TripWriteRepository : WriteRepository<Trip>, ITripWriteRepository
{
    public TripWriteRepository(AppWriteDbContext writeDbContext, ILogger<TripWriteRepository> logger) : base(writeDbContext, logger)
    {
    }

    //public override async Task<IEnumerable<Trip>> GetAllAsync()
    //{
    //    var trips = await Aggregates.AsQueryable()
    //        .Include(x => x.ScheduledStops)
    //        .ToListAsync();

    //    return trips;
    //}

    //public override async Task<Trip> GetAsync(string id)
    //{
    //    var trip = await Aggregates.AsQueryable()
    //        .Where(a => a.Id.Equals(id))
    //        .Include(x => x.ScheduledStops)
    //        .FirstOrDefaultAsync() ?? throw new KeyNotFoundException($"Aggregate of type {typeof(Trip)} could not be found using id: {id}");

    //    return trip;
    //}
}