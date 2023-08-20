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

    public override async Task<IEnumerable<Trip>> GetAllAsync()
    {
        return await Aggregates.AsQueryable().Include(a=>a.ScheduledStops).ToListAsync();
    }

    public override Task<Trip> GetAsync(string id)
    {
        return Task.FromResult(Aggregates.AsQueryable().Include(x=>x.ScheduledStops).FirstOrDefault(a=>a.Id.SequenceEqual(id)) ?? 
                               throw new KeyNotFoundException($"Aggregate of type {typeof(Trip)} could not be found using id: {id}"));
    }
}