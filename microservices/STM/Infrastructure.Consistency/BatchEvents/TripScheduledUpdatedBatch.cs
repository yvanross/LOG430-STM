using Domain.Events.AggregateEvents.Trip;

namespace Infrastructure.Consistency.BatchEvents;

public class TripScheduledUpdatedBatch
{
    public IReadOnlyCollection<TripScheduledStopsUpdated> Events { get; }

    public TripScheduledUpdatedBatch(IReadOnlyCollection<TripScheduledStopsUpdated> events)
    {
        Events = events;
    }
}