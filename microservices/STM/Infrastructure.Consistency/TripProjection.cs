using Application.QueryServices.ProjectionModels;
using Domain.Aggregates.Trip;
using Infrastructure.Consistency.BatchEvents;
using Infrastructure.ReadRepositories;
using Infrastructure.WriteRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Consistency;

public class TripProjection
{
    private readonly AppWriteDbContext _writeDbContext;
    private readonly AppReadDbContext _readDbContext;
    private readonly ILogger<TripProjection> _logger;

    public TripProjection(AppWriteDbContext writeDbContext, AppReadDbContext readDbContext , ILogger<TripProjection> logger)
    {
        _writeDbContext = writeDbContext;
        _readDbContext = readDbContext;
        _logger = logger;
    }

    public async Task HandleCreatedTrips(TripCreatedBatch batch, CancellationToken token)
    {
        var ids = batch.Events.Select(e => e.TripId).ToHashSet();

        //db
        var tripsWithModifiedStops = await _writeDbContext
            .Set<Trip>()
            .AsNoTracking()
            .Where(trip => ids.Contains(trip.Id))
            .Include(trip => trip.ScheduledStops)
            .Select(trip => new
            {
                TripId = trip.Id,
                ScheduledStops = trip.ScheduledStops
            })
            .ToListAsync(token);

        //local
        var projections = tripsWithModifiedStops
            .SelectMany(tripsWithModifiedStop => tripsWithModifiedStop.ScheduledStops
                .Select(scheduledStop => new ScheduledStopProjection()
                {
                    Id = scheduledStop.Id,
                    StopId = scheduledStop.StopId,
                    DepartureTime = scheduledStop.DepartureTime,
                    TripId = tripsWithModifiedStop.TripId
                }))
            .ToList();

        _readDbContext.ScheduledStopProjections.AddRange(projections);

        await _readDbContext.SaveChangesAsync(token);

        _logger.LogInformation($"Created {projections.Count} ScheduledStopProjections");
    }

    public async Task HandleUpdatedTrips(TripScheduledUpdatedBatch batch, CancellationToken token)
    {
        var ids = batch.Events.Select(e => e.TripId).ToHashSet();
        var updatedScheduledStops = batch.Events.SelectMany(e => e.UpdatedScheduledStopsIds).ToHashSet();

        //db
        var tripsWithModifiedStops = await _writeDbContext
            .Set<Trip>()
            .AsNoTracking()
            .Where(trip => ids.Contains(trip.Id))
            .Include(trip => trip.ScheduledStops)
            .Select(trip => new
            {
                TripId = trip.Id,
                ScheduledStops = trip.ScheduledStops
                    .Where(scheduledStop => updatedScheduledStops.Contains(scheduledStop.Id))
            })
            .ToListAsync(token);

        //local
        var projections = tripsWithModifiedStops
            .SelectMany(tripsWithModifiedStop => tripsWithModifiedStop.ScheduledStops
                .Select(scheduledStop => new ScheduledStopProjection()
                {
                    Id = scheduledStop.Id,
                    StopId = scheduledStop.StopId,
                    DepartureTime = scheduledStop.DepartureTime,
                    TripId = tripsWithModifiedStop.TripId
                }))
            .ToList();

        //db
        var existingProjectionIds = _readDbContext
            .ScheduledStopProjections
            .AsNoTracking()
            .Where(projection => updatedScheduledStops.Contains(projection.Id))
            .Select(existingProjection => existingProjection.Id)
            .ToHashSet();

        //local
        var newProjections = projections
            .Where(scheduledStop => existingProjectionIds.Contains(scheduledStop.Id) is false)
            .ToList();

        //local
        var updatedProjections = projections
            .Where(projection => existingProjectionIds.Contains(projection.Id))
            .ToList();

        _readDbContext.ScheduledStopProjections.AddRange(newProjections);

        _readDbContext.ScheduledStopProjections.UpdateRange(updatedProjections);

        await _readDbContext.SaveChangesAsync(token);

        _logger.LogInformation($"Updated {updatedProjections.Count} projections and added {newProjections.Count} new projections");
    }
}
