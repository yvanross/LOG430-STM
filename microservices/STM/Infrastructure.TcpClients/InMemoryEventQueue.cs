using Application.EventHandlers.AntiCorruption;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;

namespace Infrastructure.TcpClients;

public class InMemoryEventQueue : IPublisher, IConsumer
{
    private static readonly ConcurrentDictionary<Func<object, CancellationToken, Task>, CancellationTokenSource> Observers = new();

    private static readonly ConcurrentDictionary<Type, Channel<object>> Channels = new();

    public async Task Publish<TEvent>(TEvent message) where TEvent : class
    { 
        var channel = Channels.GetOrAdd(typeof(TEvent), Channel.CreateUnbounded<object>());

        await channel.Writer.WriteAsync(message);
    }

    /// <summary>
    /// The restriction on the TEvent type is to discriminate between ConsumeNext and Subscribe where structs come from domain events and classes come from application events
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ChannelClosedException"></exception>
    public async Task<TEvent> ConsumeNext<TEvent>(CancellationToken cancellationToken = default) where TEvent : class
    {
        var channel = await WaitForEventRegistration<TEvent>(cancellationToken);

        await foreach (var message in channel.Reader.ReadAllAsync(cancellationToken))
        {
            return (TEvent)message;
        }

        throw new ChannelClosedException($"Channel of type {typeof(TEvent)} has been closed");
    }

    /// <summary>
    /// The restriction on the TEvent type is to discriminate between ConsumeNext and Subscribe where structs come from domain events and classes come from application events
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="asyncEventHandler"></param>
    /// <param name="eventHandler"></param>
    /// <returns></returns>
    /// <exception cref="ChannelClosedException"></exception>
    public async void Subscribe<TEvent>(Func<object, CancellationToken, Task> asyncEventHandler) where TEvent : struct
    {
        var cancellationTokenSource = new CancellationTokenSource();

        var cancellationToken = cancellationTokenSource.Token;

        if (Observers.TryAdd(asyncEventHandler, cancellationTokenSource))
        {
            var channel = await WaitForEventRegistration<TEvent>(cancellationToken);

            await foreach (var message in channel.Reader.ReadAllAsync(cancellationToken))
            {
                await asyncEventHandler.Invoke(message, cancellationToken).ConfigureAwait(false);
            }

            throw new ChannelClosedException($"Channel of type {typeof(TEvent)} has been closed");
        }

        throw new InvalidOperationException("This event handler has already been subscribed");
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

    private static async Task<Channel<object>> WaitForEventRegistration<TEvent>(CancellationToken cancellationToken)
    {
        Channel<object> channel;

        while (true)
        {
            if (Channels.TryGetValue(typeof(TEvent), out channel!)) break;

            await Task.Delay(100, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();
        }

        return channel;
    }
}