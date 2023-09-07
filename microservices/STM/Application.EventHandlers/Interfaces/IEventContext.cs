namespace Application.EventHandlers.Interfaces;

public interface IEventContext
{
    Task<T?> TryGetAsync<T>() where T : Event;

    Task AddOrUpdateAsync<T>(T @event) where T : Event;
}