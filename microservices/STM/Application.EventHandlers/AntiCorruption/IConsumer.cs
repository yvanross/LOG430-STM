namespace Application.EventHandlers.AntiCorruption;

public interface IConsumer
{
    Task<TEvent?> Consume<TEvent>() where TEvent : class;
}