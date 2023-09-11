namespace Application.EventHandlers.Interfaces;

public interface IPublisher
{
    Task Publish<TEvent>(TEvent message) where TEvent : Event;
}