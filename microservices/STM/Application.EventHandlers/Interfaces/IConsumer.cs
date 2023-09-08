using Application.EventHandlers.Messaging.PipeAndFilter;
using Microsoft.Extensions.Logging;

namespace Application.EventHandlers.Interfaces;

public interface IConsumer
{
    Task<TMessage> ConsumeNext<TMessage>(CancellationToken token = default) where TMessage : class;

    void Subscribe<TEvent>(
        Func<TEvent, CancellationToken, Task> asyncEventHandler,
        ILogger logger) where TEvent : class;

    void Subscribe<TEvent, TResult>(
        Func<TResult, CancellationToken, Task> asyncEventHandler,
        ILogger logger,
        params Funnel[] funnels) 
        where TResult : class 
        where TEvent : class;

    void UnSubscribe<TResult>(Func<TResult, CancellationToken, Task> asyncEventHandler) where TResult : class;
}