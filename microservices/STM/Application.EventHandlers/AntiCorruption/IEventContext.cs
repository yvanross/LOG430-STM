namespace Application.EventHandlers.AntiCorruption;

public interface IEventContext
{
    Task<T?> TryGetAsync<T>() where T : Event;

    Task AddOrUpdateAsync<T>(T @event) where T : Event;
}