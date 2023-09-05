using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Channels;
using Application.EventHandlers.AntiCorruption;
using Application.EventHandlers.Messaging.PipeAndFilter;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application.EventHandlers.Messaging;

public class InMemoryEventQueue : IPublisher, IConsumer
{
    private readonly IServiceProvider? _serviceProvider;

    private static readonly ConcurrentDictionary<Type, MethodInfo> HandlerMethods = new();

    private static readonly ConcurrentDictionary<Delegate, CancellationTokenSource> Observers = new();

    private static readonly ConcurrentDictionary<Type, List<Channel<object>>> Channels = new();

    public InMemoryEventQueue(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    ///  Consume the next event of type TEvent
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ChannelClosedException"></exception>
    public async Task<TEvent> ConsumeNext<TEvent>(CancellationToken cancellationToken = default) where TEvent : class
    {
        var channel = Channel.CreateUnbounded<object>();
        var type = typeof(TEvent);

        var channels = Channels.GetOrAdd(type, _ => new List<Channel<object>>());
        channels.Add(channel);

        await foreach (var message in channel.Reader.ReadAllAsync(cancellationToken)) return (TEvent)message;

        throw new ChannelClosedException($"Channel of type {typeof(TEvent)} has been closed");
    }

    public void Subscribe<TEvent>(Func<TEvent, CancellationToken, Task> asyncEventHandler, ILogger logger) where TEvent : class
    {
        Subscribe<TEvent, TEvent>(asyncEventHandler, logger);
    }

    /// <summary>
    ///     Subscribes to an event of type TEvent and processes it through the given funnels, returning a result of type
    ///     TResult.
    /// </summary>
    public async void Subscribe<TEvent, TResult>(
        Func<TResult, CancellationToken, Task> asyncEventHandler,
        ILogger logger,
        params Funnel[] funnels) where TResult : class
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

                var pipeline = new Pipeline<TEvent, TResult>(funnels, channel.Reader, cancellationTokenSource, logger);

                await foreach (var message in pipeline.Process().ReadAllAsync(cancellationToken))
                {
                    var result = message as TResult ??
                                 throw new InvalidOperationException(
                                     $"Expected type {typeof(TResult)} but got {message.GetType()}");

                    await asyncEventHandler.Invoke(result, cancellationToken).ConfigureAwait(false);
                }

                throw new ChannelClosedException($"Channel of type {typeof(TEvent)} has been closed");
            }

            throw new InvalidOperationException("This event handler has already been subscribed");
        }
        catch (OperationCanceledException)
        {
            // exit gracefully
        }
    }

    public void UnSubscribe<TResult>(Func<TResult, CancellationToken, Task> asyncEventHandler) where TResult : class
    {
        if (Observers.TryRemove(asyncEventHandler, out var cancellationTokenSource))
            cancellationTokenSource.Cancel();
        else
            throw new InvalidOperationException("This event handler has not been subscribed");
    }

    public async Task Publish<TEvent>(TEvent message) where TEvent : Event
    {
        var type = typeof(TEvent);

        if (Channels.TryGetValue(type, out var channels))
            foreach (var channel in channels)
                await channel.Writer.WriteAsync(message);

        await DispatchAsync(message);
    }

    private async Task DispatchAsync(Event applicationEvent)
    {
        if (_serviceProvider is null) return;

        var eventType = applicationEvent.GetType();

        var handleMethod = GetHandleMethod(eventType);

        await InvokeHandler(applicationEvent, handleMethod);
    }

    private MethodInfo GetHandleMethod(Type eventType)
    {
        if (HandlerMethods.TryGetValue(eventType, out var handleMethod)) return handleMethod;

        var handlerType = typeof(IApplicationEventHandler<>).MakeGenericType(eventType);

        var handlerMethods = handlerType.GetMethods().Where(m => m.Name == "HandleAsync").ToList();

        if (handlerMethods.Count != 1)
            throw new InvalidOperationException(
                $"Expected exactly one HandleAsync method on {handlerType.Name}, but found {handlerMethods.Count}.");

        handleMethod = handlerMethods.Single();

        HandlerMethods[eventType] = handleMethod;

        return handleMethod;
    }

    private async Task InvokeHandler(Event applicationEvent, MethodBase handleMethod)
    {
        using var scope = _serviceProvider.CreateScope();

        var handler = scope.ServiceProvider.GetService(
            (handleMethod ??
             throw new InvalidOperationException(
                 $"No handler method found for the application event type {applicationEvent.GetType().Name}."))
            .DeclaringType ??
            throw new InvalidOperationException(
                $"Declaring type for the handle method not found for the application event type {applicationEvent.GetType().Name}."));

        if (handler == null) return;

        await ((Task)handleMethod.Invoke(handler, new object[] { applicationEvent })!)!;
    }
}