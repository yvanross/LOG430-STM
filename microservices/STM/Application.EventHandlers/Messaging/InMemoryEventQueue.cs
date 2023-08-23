using Application.EventHandlers.AntiCorruption;
using Application.EventHandlers.Messaging.PipeAndFilter;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace Application.EventHandlers.Messaging;

public class InMemoryEventQueue : IPublisher, IConsumer
{
    private static readonly ConcurrentDictionary<Delegate, CancellationTokenSource> Observers = new();

    //private static readonly ConcurrentDictionary<Type, Channel<object>> Channels = new();

    private static readonly ConcurrentDictionary<Type, List<Channel<object>>> Channels = new();

    public async void Publish<TEvent>(TEvent message)
    {
        var type = typeof(TEvent);

        if (Channels.TryGetValue(type, out var channels))
        {
            foreach (var channel in channels)
            {
                await channel.Writer.WriteAsync(message);
            }
        }
    }

    //public async Task Publish<TEvent>(TEvent message)
    //{
    //    var channel = Channels.GetOrAdd(typeof(TEvent), Channel.CreateUnbounded<object>());

    //    await channel.Writer.WriteAsync(message);
    //}

    /// <summary>
    /// The restriction on the TEvent type is to discriminate between ConsumeNext and Subscribe where structs come from domain events and classes come from application events
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ChannelClosedException"></exception>
    public async Task<TEvent> ConsumeNext<TEvent>(CancellationToken cancellationToken = default) where TEvent : class
    {
        //var channel = await WaitForEventRegistration<TEvent>(cancellationToken);
        var channel = Channel.CreateUnbounded<object>();
        var type = typeof(TEvent);

        var channels = Channels.GetOrAdd(type, _ => new List<Channel<object>>());
        channels.Add(channel);

        await foreach (var message in channel.Reader.ReadAllAsync(cancellationToken))
        {
            return (TEvent)message;
        }

        throw new ChannelClosedException($"Channel of type {typeof(TEvent)} has been closed");
    }

    /// <summary>
    /// Subscribes to an event of type TEvent and processes it through the given funnels, returning a result of type TResult.
    /// </summary>
    public async void Subscribe<TEvent, TResult>(
        Func<TResult, CancellationToken, Task> asyncEventHandler,
        ILogger logger,
        params Funnel [] funnels) where TResult : class
    {
        try
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var cancellationToken = cancellationTokenSource.Token;

            if (Observers.TryAdd(asyncEventHandler, cancellationTokenSource))
            {
                var channel = Channel.CreateUnbounded<object>();
                var type = typeof(TEvent);

                var channels = Channels.GetOrAdd(type, _ => new List<Channel<object>>());
                channels.Add(channel);

                //   var channel = await WaitForEventRegistration<TEvent>(cancellationToken);

                var pipeline = new Pipeline<TEvent, TResult>(funnels, channel.Reader, cancellationTokenSource, logger);

                await foreach (var message in pipeline.Process().ReadAllAsync(cancellationToken))
                {
                    var result = message as TResult ?? throw new InvalidOperationException($"Expected type {typeof(TResult)} but got {message.GetType()}");

                    await asyncEventHandler.Invoke(result, cancellationToken).ConfigureAwait(false);
                }

                throw new ChannelClosedException($"Channel of type {typeof(TEvent)} has been closed");
            }

            throw new InvalidOperationException("This event handler has already been subscribed");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void UnSubscribe(Func<object, CancellationToken, Task> asyncEventHandler)
    {
        if (Observers.TryRemove(asyncEventHandler, out var cancellationTokenSource))
        {
            cancellationTokenSource.Cancel();
        }
        else
            throw new InvalidOperationException("This event handler has not been subscribed");
    }

    //private static async Task<Channel<object>> WaitForEventRegistration<TEvent>(CancellationToken cancellationToken)
    //{
    //    Channel<object> channel;

    //    while (true)
    //    {
    //        var type = typeof(TEvent);

    //        if (Channels.TryGetValue(type, out channel!)) break;

    //        await Task.Delay(100, cancellationToken);

    //        cancellationToken.ThrowIfCancellationRequested();
    //    }

    //    return channel;
    //}
}