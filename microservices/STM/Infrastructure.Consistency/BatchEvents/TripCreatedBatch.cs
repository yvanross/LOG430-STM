using Domain.Events.AggregateEvents.Trip;

namespace Infrastructure.Consistency.BatchEvents;

public class TripCreatedBatch
{
    public IReadOnlyCollection<TripCreated> Events { get; }

    public TripCreatedBatch(IReadOnlyCollection<TripCreated> events)
    {
        Events = events;
    }
}