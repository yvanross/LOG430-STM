namespace Application.EventHandlers.Interfaces;

public interface IEventPublisher
{
    Task Publish<TEvent>(TEvent message) where TEvent : Event;
}