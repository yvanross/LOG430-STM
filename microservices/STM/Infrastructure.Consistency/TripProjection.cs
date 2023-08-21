using Application.EventHandlers.AntiCorruption;
using Application.QueryServices.ProjectionModels;
using Domain.Aggregates.Trip;
using Domain.Events.AggregateEvents.Trip;
using Infrastructure.ReadRepositories;
using Infrastructure.WriteRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Consistency;

public class TripProjection : BackgroundService
{
    private readonly IConsumer _consumer;
    private readonly AppReadDbContext _readContext;
    private readonly AppWriteDbContext _writeDbContext;
    private readonly ILogger<TripProjection> _logger;

    public TripProjection(IConsumer consumer, AppReadDbContext readContext, AppWriteDbContext writeDbContext, ILogger<TripProjection> logger)
    {
        _consumer = consumer;
        _readContext = readContext;
        _writeDbContext = writeDbContext;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe<TripScheduledStopsUpdated>(HandleUpdate);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _consumer.UnSubscribe(HandleUpdate);

        base.Dispose();
    }

    private async Task HandleUpdate(object message, CancellationToken stoppingToken)
    {
        if (message is not TripScheduledStopsUpdated tripScheduledStopsUpdated)
        {
            _logger.LogError($"Received message of type {message.GetType()} but expected {typeof(TripScheduledStopsUpdated)}");
            
            return;
        }
        // TODO: This is a very naive implementation. It will not scale well.

        var tripsWithModifiedStops = await _writeDbContext
            .Set<Trip>()
            .AsNoTracking()
            .Include(trip => trip.ScheduledStops)
            .Select(trip => new
            {
                TripId = trip.Id,
                ScheduledStops = trip.ScheduledStops
                    .Where(scheduledStop => tripScheduledStopsUpdated.UpdatedScheduledStopsIds.Contains(scheduledStop.Id))
            })
            .ToListAsync(cancellationToken: stoppingToken);

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

        var existingProjections = await _readContext
            .ScheduledStopProjections
            .AsNoTracking()
            .Where(projection => projection.Id.Equals(tripScheduledStopsUpdated.UpdatedScheduledStopsIds))
            .ToListAsync(stoppingToken);

        var newProjections = projections
            .Where(scheduledStop => existingProjections
                .Any(projectedScheduledStop => projectedScheduledStop.Id.Equals(scheduledStop.Id)) is false)
            .ToList();

        var updatedProjections = projections.Except(newProjections).ToList();

        _readContext.ScheduledStopProjections.AddRange(newProjections);

        _readContext.ScheduledStopProjections.UpdateRange(updatedProjections);

        await _readContext.SaveChangesAsync(stoppingToken);

        _logger.LogInformation($"Updated {updatedProjections.Count} projections and added {newProjections.Count} new projections");
    }
}