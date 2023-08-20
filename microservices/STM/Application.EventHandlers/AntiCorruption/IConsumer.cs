namespace Application.EventHandlers.AntiCorruption;

public interface IConsumer
{
    Task<TMessage> ConsumeNext<TMessage>(CancellationToken token = default) where TMessage : class;
}