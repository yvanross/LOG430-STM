
namespace Domain.Events.Interfaces;

public interface IDomainEvent
{
    /// <summary>
    /// Allows reducing the number of events of the same type when they are raised in the same transaction.
    /// Not using generics because the logic filtering event types is in a higher layer, forcing manual type checking when overriding in concrete events.
    /// This is a tradeoff to avoid back pressure on the event handlers.
    /// </summary>
    /// <returns></returns>
    Func<IEnumerable<IDomainEvent>, IEnumerable<IDomainEvent>> GetEventReduceBehavior()
        // if not overriden, default behavior is no need to merge events
        => events => events;

    /// <summary>
    /// Allows merging the events of the same type into fewer events (batch) when they are raised in the same transaction.
    /// </summary>
    /// <returns></returns>
    Func<IEnumerable<IDomainEvent>, IEnumerable<IDomainEvent>> GetEventBatchBehavior()
        // if not overriden, default behavior is no need to merge events
        => events => events;
}