namespace Application.EventHandlers.AntiCorruption;

public interface IPublisher
{
    void Publish<TEvent>(TEvent message);
}