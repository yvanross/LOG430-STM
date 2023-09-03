using Application.CommandServices.Repositories;
using Application.Common.Extensions;
using Domain.Aggregates.Trip;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.WriteRepositories;

public class TripWriteRepository : WriteRepository<Trip>, ITripWriteRepository
{
    private readonly AppWriteDbContext _writeDbContext;

    public TripWriteRepository(AppWriteDbContext writeDbContext, ILogger<TripWriteRepository> logger) : base(writeDbContext, logger)
    {
        _writeDbContext = writeDbContext;
    }

    public override async Task AddAllAsync(IEnumerable<Trip> aggregates)
    {
        try
        {
            if (WriteDbContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
            {
                await BatchInsertAsync(aggregates);
            }
            else
                await Aggregates.AddRangeAsync(aggregates);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void Update(Trip trip)
    {
        //the in memory provider is configured differently to avoid shadow props making this step necessary
        if (WriteDbContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
            return;

        var trackedTrip = Aggregates.Attach(trip);

        foreach (var scheduledStop in trip.ScheduledStops)
        {
            var trackedScheduledStop = _writeDbContext.Entry(scheduledStop);

            if (trackedScheduledStop.State == EntityState.Detached)
            {
                _writeDbContext.Set<ScheduledStop>().Attach(scheduledStop);
            }

            trackedScheduledStop.Property("TripId").CurrentValue = trip.Id;
        }

        trackedTrip.State = EntityState.Modified;
    }

    private async Task BatchInsertAsync(IEnumerable<Trip> aggregates)
    {
        List<Trip> batch = new List<Trip>(100000);

        foreach (Trip trip in aggregates)
        {
            batch.Add(trip);

            if (batch.Count >= 100000)
            {
                await WriteDbContext.BulkInsertAsync(batch, operation =>
                {
                    operation.BatchSize = 1000;
                    operation.BatchTimeout = 360;
                });

                batch.Clear();
            }
        }

        if (batch.Count > 0)
        {
            await WriteDbContext.BulkInsertAsync(batch, operation =>
            {
                operation.BatchSize = 1000;
                operation.BatchTimeout = 360;
            });
        }
    }

    public override async Task<Trip> GetAsync(string id)
    {
        return WriteDbContext.Database.ProviderName!.Equals("Microsoft.EntityFrameworkCore.InMemory") ?
            await Aggregates
            .Include(x => x.ScheduledStops)
            .FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException($"Aggregate of type {typeof(Trip)} could not be found using id: {id}") :
            await base.GetAsync(id);
    }

    public override async Task<IEnumerable<Trip>> GetAllAsync(params string[] ids)
    {
        return await(ids.IsEmpty() ?
            Aggregates.Include(x=>x.ScheduledStops).ToListAsync() :
            Aggregates.Where(a => ids.Contains(a.Id)).Include(x => x.ScheduledStops).ToListAsync());
    }
}