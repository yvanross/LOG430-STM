using Application.EventHandlers.Messaging;
using Application.EventHandlers.Messaging.PipeAndFilter;
using Domain.Events.AggregateEvents.Trip;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace Application.EventHandlers.AntiCorruption;

public interface IConsumer
{
    Task<TMessage> ConsumeNext<TMessage>(CancellationToken token = default) where TMessage : class;

    void Subscribe<TEvent, TResult>(
        Func<TResult, CancellationToken, Task> asyncEventHandler,
        ILogger logger,
        params Funnel[] funnels) where TResult : class;

    void UnSubscribe(Func<object, CancellationToken, Task> asyncEventHandler);
}