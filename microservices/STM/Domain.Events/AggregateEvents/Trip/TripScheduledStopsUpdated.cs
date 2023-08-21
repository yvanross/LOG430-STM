using Domain.Events.Interfaces;
using System.Linq;

namespace Domain.Events.AggregateEvents.Trip;

public readonly record struct TripScheduledStopsUpdated(string TripId, params string[] UpdatedScheduledStopsIds) : IDomainEvent
{
    public Func<IEnumerable<IDomainEvent>, IEnumerable<IDomainEvent>> GetEventReduceBehavior()
    {
        return events =>
        {
            if (events.All(instance => instance is TripScheduledStopsUpdated))
            {
                var allEvents = events.Cast<TripScheduledStopsUpdated>().ToList();

                var mergedEvents = allEvents
                    .GroupBy(instance => instance.TripId)
                    .Select(group =>
                        new TripScheduledStopsUpdated(group.Key, group.SelectMany(instance => instance.UpdatedScheduledStopsIds)
                            .ToArray()));

                return mergedEvents.Cast<IDomainEvent>().ToList();
            }

            throw new InvalidOperationException();
        };
    }
}