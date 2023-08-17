using Application.EventHandlers.AntiCorruption;
using System.Collections.Concurrent;

namespace Infrastructure.TcpClients;

//This is bad
public class InMemoryEventQueue : IPublisher, IConsumer
{
    private readonly ConcurrentDictionary<Type, ConcurrentQueue<object>> _queue = new();

    public Task Publish<TEvent>(TEvent message) where TEvent : class
    {
        _queue.AddOrUpdate(typeof(TEvent), _ => new ConcurrentQueue<object>(), (_, queue) =>
        {
            queue.Enqueue(message);

            return queue;
        });

        return Task.CompletedTask;
    }

    public Task<TEvent?> Consume<TEvent>() where TEvent : class
    {
        if (_queue.TryGetValue(typeof(TEvent), out var queue))
        {
            if (queue.TryDequeue(out var message))
            {
                return Task.FromResult((TEvent?)message);
            }
        }

        return Task.FromResult<TEvent?>(null);
    }
}