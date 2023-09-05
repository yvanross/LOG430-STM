using Application.CommandServices.Repositories;
using Application.Common.Extensions;
using Domain.Aggregates.Trip;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.WriteRepositories;

public class TripWriteRepository : WriteRepository<Trip>, ITripWriteRepository
{
    private readonly AppWriteDbContext _writeDbContext;

    public TripWriteRepository(AppWriteDbContext writeDbContext, ILogger<TripWriteRepository> logger) : base(
        writeDbContext, logger)
    {
        _writeDbContext = writeDbContext;
    }

    public void Update(Trip trip)
    {
        //the in memory provider is configured differently to avoid shadow props, it doesn't track updates to child collections in this configuration
        //so we need to manually attach the child collection and set the foreign key
        //highly advise against using an in memory provider (we use it to demonstrate how bad it is)
        if (DatabaseIsInMemory)
        {
            var trackedTrip = Aggregates.Attach(trip);

            foreach (var scheduledStop in trip.ScheduledStops)
            {
                var trackedScheduledStop = _writeDbContext.Entry(scheduledStop);

                if (trackedScheduledStop.State == EntityState.Detached)
                    _writeDbContext.Set<ScheduledStop>().Attach(scheduledStop);

                trackedScheduledStop.Property("TripId").CurrentValue = trip.Id;
            }

            trackedTrip.State = EntityState.Modified;
        }
    }

    public override async Task<Trip> GetAsync(string id)
    {
        var trip = DatabaseIsInMemory ? 
            await Aggregates
                                            .Include(x => x.ScheduledStops)
                                            .FirstOrDefaultAsync(x => x.Id == id)
                                        ?? throw new KeyNotFoundException($"Aggregate of type {typeof(Trip)} could not be found using id: {id}")
            : await base.GetAsync(id);

        trip.InvalidateState();

        return trip;
    }

    public override async Task<IEnumerable<Trip>> GetAllAsync(params string[] ids)
    {
        var trips = await (ids.IsEmpty()
            ? Aggregates.Include(x => x.ScheduledStops).ToListAsync()
            : Aggregates.Where(a => ids.Contains(a.Id)).Include(x => x.ScheduledStops).ToListAsync());
        
        trips.ForEach(x => x.InvalidateState());

        return trips;
    }

    public override async Task AddAllAsync(IEnumerable<Trip> aggregates)
    {
        if (DatabaseIsInMemory)
            await Aggregates.AddRangeAsync(aggregates);
        else
            await BatchInsertAsync(aggregates);
    }

    public override async Task UpdateAllAsync(IEnumerable<Trip> aggregates)
    {
        if (DatabaseIsInMemory)
            Aggregates.UpdateRange(aggregates);
        else
            await BatchUpdateAsync(aggregates);

    }

    private async Task BatchInsertAsync(IEnumerable<Trip> aggregates)
    {
        var batch = new List<Trip>(100000);

        foreach (var trip in aggregates)
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
            await WriteDbContext.BulkInsertAsync(batch, operation =>
            {
                operation.BatchSize = 1000;
                operation.BatchTimeout = 360;
            });
    }

    private async Task BatchUpdateAsync(IEnumerable<Trip> aggregates)
    {
        var batch = new List<Trip>(100000);

        foreach (var trip in aggregates)
        {
            batch.Add(trip);

            if (batch.Count >= 100000)
            {
                await WriteDbContext.BulkUpdateAsync(batch, operation =>
                {
                    operation.BatchSize = 1000;
                    operation.BatchTimeout = 360;
                });

                batch.Clear();
            }
        }

        if (batch.Count > 0)
            await WriteDbContext.BulkUpdateAsync(batch, operation =>
            {
                operation.BatchSize = 1000;
                operation.BatchTimeout = 360;
            });
    }
}