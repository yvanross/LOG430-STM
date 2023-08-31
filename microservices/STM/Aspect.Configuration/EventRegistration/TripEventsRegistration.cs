using System.Collections.Concurrent;
using Application.EventHandlers.AntiCorruption;
using Application.EventHandlers.Messaging.PipeAndFilter;
using Domain.Events.AggregateEvents.Trip;
using Infrastructure.Consistency.BatchEvents;
using Infrastructure.Consistency;

namespace Aspect.Configuration.EventRegistration;

public class TripEventsRegistration
{
    private readonly IConsumer _consumer;
    private readonly TripProjection _tripProjection;
    private readonly ILogger<TripEventsRegistration> _logger;

    public TripEventsRegistration(IConsumer consumer, TripProjection tripProjection, ILogger<TripEventsRegistration> logger)
    {
        _consumer = consumer;
        _tripProjection = tripProjection;
        _logger = logger;
    }

    internal void SetupTripScheduledStopUpdatedEventHandlerAndBatching()
    {
        _consumer.Subscribe<TripScheduledStopsUpdated, TripScheduledUpdatedBatch>(_tripProjection.HandleUpdatedTrips, _logger,
            new Funnel(
                async (reader, writer, cancellationToken) =>
                {
                    ConcurrentQueue<TripScheduledStopsUpdated> incomingEvents = new();

                    var timer = new Timer(_ =>
                    {
                        Dictionary<string, TripScheduledStopsUpdated> batch = new();

                        while (incomingEvents.TryDequeue(out var incomingEvent))
                        {
                            if (batch.TryGetValue(incomingEvent.TripId, out var value))
                            {
                                foreach (var UpdatedScheduledStopsId in incomingEvent.UpdatedScheduledStopsIds)
                                {
                                    value.UpdatedScheduledStopsIds.Add(UpdatedScheduledStopsId);
                                }
                            }
                            else
                            {
                                batch.Add(incomingEvent.TripId, incomingEvent);
                            }
                        }

                        if (batch.Any()) writer.WriteAsync(new TripScheduledUpdatedBatch(batch.Values), cancellationToken);
                    }, null, TimeSpan.Zero, TimeSpan.FromSeconds(2.5));

                    await foreach (var @event in reader.ReadAllAsync(cancellationToken))
                    {
                        incomingEvents.Enqueue(@event);
                    }
                }, typeof(TripScheduledStopsUpdated), typeof(TripScheduledUpdatedBatch)));
    }

    internal void SetupTripCreatedEventHandlerAndBatching()
    {
        _consumer.Subscribe<TripCreated, TripCreatedBatch>(_tripProjection.HandleCreatedTrips, _logger, new Funnel(
            async (reader, writer, cancellationToken) =>
            {
                ConcurrentQueue<TripCreated> incomingEvents = new();

                var timer = new Timer(_ =>
                {
                    List<TripCreated> batch = new();

                    while (incomingEvents.TryDequeue(out var incomingEvent))
                    {
                        batch.Add(incomingEvent);
                    }

                    if (batch.Any()) writer.WriteAsync(new TripCreatedBatch(batch), cancellationToken);
                }, null, TimeSpan.Zero, TimeSpan.FromSeconds(2.5));

                await foreach (var @event in reader.ReadAllAsync(cancellationToken))
                {
                    incomingEvents.Enqueue(@event);
                }
            }, typeof(TripCreated), typeof(TripCreatedBatch)));
    }
}