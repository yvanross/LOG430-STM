using Application.EventHandlers.AntiCorruption;
using System.Collections.Concurrent;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace Infrastructure.TcpClients;

//This is bad in production
public class InMemoryEventQueue : IPublisher, IConsumer
{
    private readonly ILogger<InMemoryEventQueue> _logger;
    private static readonly ConcurrentDictionary<Type, Channel<object>> Channels = new();

    public InMemoryEventQueue(ILogger<InMemoryEventQueue> logger)
    {
        _logger = logger;
    }

    public Task Publish<TEvent>(TEvent message) where TEvent : class
    { 
        var channel = Channels.GetOrAdd(typeof(TEvent), Channel.CreateUnbounded<object>());

        channel.Writer.TryWrite(message);

        return Task.CompletedTask;
    }

    public async Task<TEvent> ConsumeNext<TEvent>(CancellationToken cancellationToken = default) where TEvent : class
    {
        Channel<object> channel;

        while (true)
        {
            if (Channels.TryGetValue(typeof(TEvent), out channel)) break;
            
            await Task.Delay(100, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();
        }

        await foreach (var message in channel.Reader.ReadAllAsync(cancellationToken))
        {
            return (TEvent)message;
        }

        throw new ChannelClosedException($"Channel of type {typeof(TEvent)} has been closed");
    }
}