using System.Threading.Channels;
using Application.EventHandlers.AntiCorruption;
using Application.EventHandlers.Messaging.PipeAndFilter;
using Application.QueryServices.ProjectionModels;
using Domain.Aggregates.Trip;
using Domain.Common.Seedwork.Abstract;
using Domain.Events.AggregateEvents.Trip;
using Domain.Events.Interfaces;
using Infrastructure.Consistency.BatchEvents;
using Infrastructure.ReadRepositories;
using Infrastructure.WriteRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Consistency;

public class TripProjection : IDomainEventHandler<TripScheduledStopsUpdated>, IDomainEventHandler<TripCreated>
{
    private readonly IConsumer _consumer;
    private readonly AppWriteDbContext _writeDbContext;
    private readonly AppReadDbContext _readDbContext;
    private readonly ILogger<TripProjection> _logger;

    public TripProjection(IConsumer consumer, AppWriteDbContext writeDbContext, AppReadDbContext readDbContext , ILogger<TripProjection> logger)
    {
        _consumer = consumer;
        _writeDbContext = writeDbContext;
        _readDbContext = readDbContext;
        _logger = logger;
    }

    //Duplication of code because it represents different intent and so it is not a violation of DRY

    public async Task HandleAsync(TripScheduledStopsUpdated domainEvent)
    {
        var tripsWithModifiedStops = await _writeDbContext
            .Set<Trip>()
            .AsNoTracking()
            .Include(trip => trip.ScheduledStops)
            .Select(trip => new
            {
                TripId = trip.Id,
                ScheduledStops = trip.ScheduledStops
                    .Where(scheduledStop => domainEvent.UpdatedScheduledStopsIds.Contains(scheduledStop.Id))
            })
            .ToListAsync();

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

        var existingProjections = await _readDbContext
            .ScheduledStopProjections
            .AsNoTracking()
            .Where(projection => projection.Id.Equals(domainEvent.UpdatedScheduledStopsIds))
            .ToListAsync();

        var newProjections = projections
            .Where(scheduledStop => existingProjections
                .Any(projectedScheduledStop => projectedScheduledStop.Id.Equals(scheduledStop.Id)) is false)
            .ToList();

        var updatedProjections = projections.Except(newProjections).ToList();

        _readDbContext.ScheduledStopProjections.AddRange(newProjections);

        _readDbContext.ScheduledStopProjections.UpdateRange(updatedProjections);

        await _readDbContext.SaveChangesAsync();

        _logger.LogInformation($"Updated {updatedProjections.Count} projections and added {newProjections.Count} new projections");
    }

    public async Task HandleAsync(TripCreated domainEvent)
    {
        var aggregate = await _writeDbContext
            .Set<Trip>()
            .FindAsync(domainEvent.TripId);

        var tripsWithModifiedStops = await _writeDbContext
            .Set<Trip>()
            .AsNoTracking()
            .Include(trip => trip.ScheduledStops
                .Where(scheduledStop => domainEvent.UpdatedScheduledStopsIds.Contains(scheduledStop.Id)))
            .Select(trip => new
            {
                TripId = trip.Id,
                ScheduledStops = trip.ScheduledStops
            })
            .ToListAsync();

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

        var existingProjections = await _readDbContext
            .ScheduledStopProjections
            .AsNoTracking()
            .Where(projection => projection.Id.Equals(domainEvent.UpdatedScheduledStopsIds))
            .ToListAsync();

        var newProjections = projections
            .Where(scheduledStop => existingProjections
                .Any(projectedScheduledStop => projectedScheduledStop.Id.Equals(scheduledStop.Id)) is false)
            .ToList();

        var updatedProjections = projections.Except(newProjections).ToList();

        _readDbContext.ScheduledStopProjections.AddRange(newProjections);

        _readDbContext.ScheduledStopProjections.UpdateRange(updatedProjections);

        await _readDbContext.SaveChangesAsync();

        _logger.LogInformation($"Updated {updatedProjections.Count} projections and added {newProjections.Count} new projections");
    }

    //protected override Task ExecuteAsync(CancellationToken stoppingToken)
    //{
    //    var funnel = new Func<ChannelReader<TripScheduledStopsUpdated>, ChannelWriter<TripScheduledUpdatedBatch>, Task>((reader, writer) =>
    //    {
    //        return Task.CompletedTask;
    //    });

    //    _consumer.Subscribe<TripScheduledStopsUpdated, TripScheduledUpdatedBatch>(AsyncEventHandler, new Funnel());
    //}

    private Task AsyncEventHandler(TripScheduledStopsUpdated @event, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}
