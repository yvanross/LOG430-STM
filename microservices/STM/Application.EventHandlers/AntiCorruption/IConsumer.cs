using Domain.Events.AggregateEvents.Trip;
using System.Threading.Channels;

namespace Application.EventHandlers.AntiCorruption;

public interface IConsumer
{
    Task<TMessage> ConsumeNext<TMessage>(CancellationToken token = default) where TMessage : class;


    /// <summary>
    /// The restriction on the TEvent type is to discriminate between ConsumeNext and Subscribe where structs come from domain events and classes come from application events
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="asyncEventHandler"></param>
    /// <returns></returns>
    /// <exception cref="ChannelClosedException"></exception>
    void Subscribe<TEvent>(Func<object, CancellationToken, Task> asyncEventHandler) where TEvent : struct;
    
    void UnSubscribe(Func<object, CancellationToken, Task> asyncEventHandler);
}