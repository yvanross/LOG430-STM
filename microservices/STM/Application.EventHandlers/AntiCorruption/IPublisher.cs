namespace Application.EventHandlers.AntiCorruption;

public interface IPublisher
{
    Task Publish<TEvent>(TEvent message);
}