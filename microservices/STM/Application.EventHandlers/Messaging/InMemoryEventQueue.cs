using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Channels;
using Application.EventHandlers.Interfaces;
using Application.EventHandlers.Messaging.PipeAndFilter;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application.EventHandlers.Messaging;

public class InMemoryEventQueue : IEventPublisher, IConsumer
{
    private readonly IServiceProvider? _serviceProvider;
    private readonly ILogger<InMemoryEventQueue>? _logger;

    private static readonly ConcurrentDictionary<Type, MethodInfo> HandlerMethods = new();

    private static readonly ConcurrentDictionary<Delegate, CancellationTokenSource> Observers = new();

    private static readonly ConcurrentDictionary<Type, List<Channel<object>>> Channels = new();

    private static readonly ConcurrentDictionary<Type, SemaphoreSlim> SemaphoreSlims = new();

    public InMemoryEventQueue(IServiceProvider serviceProvider, ILogger<InMemoryEventQueue> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
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
        try
        {
            var channel = Channel.CreateUnbounded<object>();

            await SafeAddChannelOfType<TEvent>(channel, cancellationToken);

            TEvent? result = null;

            await foreach (var message in channel.Reader.ReadAllAsync(cancellationToken))
            {
                result = (TEvent)message;

                break;
            }

            if (result is null) throw new ChannelClosedException($"Channel of type {typeof(TEvent)} has been closed or result was null");

            await SafeRemoveChannelOfType<TEvent>(channel, CancellationToken.None);

            return result;
        }
        catch (Exception e)
        {
            _logger?.LogError(e, $"Error consuming next event of type {typeof(TEvent)}");
            throw;
        }
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
        params Funnel[] funnels) 
        where TResult : class
        where TEvent : class
    {
        var channel = Channel.CreateUnbounded<object>();

        var cancellationTokenSource = new CancellationTokenSource();

        var cancellationToken = cancellationTokenSource.Token;

        try
        {
            if (Observers.TryAdd(asyncEventHandler, cancellationTokenSource))
            {
                await SafeAddChannelOfType<TEvent>(channel, cancellationToken);

                var pipeline = new Pipeline<TEvent, TResult>(funnels, channel.Reader, cancellationTokenSource, logger);

                await foreach (var message in pipeline.Process().ReadAllAsync(cancellationToken).ConfigureAwait(false))
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
        catch (Exception e)
        {
            _logger?.LogError(e, "Error in event handler");
            throw;
        }
        finally
        {
            await SafeRemoveChannelOfType<TEvent>(channel, CancellationToken.None);
        }
    }

    public void UnSubscribe<TResult>(Func<TResult, CancellationToken, Task> asyncEventHandler) where TResult : class
    {
        try
        {
            if (Observers.TryRemove(asyncEventHandler, out var cancellationTokenSource))
                cancellationTokenSource.Cancel();
            else
                throw new InvalidOperationException("This event handler has not been subscribed");
        }
        catch (Exception e)
        {
            _logger?.LogError(e, " Error Unsubscribing from event");
            throw;
        }
        
    }

    public async Task Publish<TEvent>(TEvent message) where TEvent : Event
    {
        try
        {
            var type = typeof(TEvent);

            if (Channels.TryGetValue(type, out var channels))
            {
                var semaphores = SemaphoreSlims.GetOrAdd(type, _ => new SemaphoreSlim(1));

                await semaphores.WaitAsync();

                foreach (var channel in channels)
                    await channel.Writer.WriteAsync(message);

                semaphores.Release();
            }
                

            await DispatchAsync(message);
        }
        catch (Exception e)
        {
            _logger?.LogError(e, "Error publishing event");
            throw;
        }
        
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

    private async Task SafeAddChannelOfType<TEvent>(Channel<object> channel, CancellationToken cancellationToken) where TEvent : class
    {
        var type = typeof(TEvent);

        var channels = Channels.GetOrAdd(type, _ => new List<Channel<object>>());
        var semaphore = SemaphoreSlims.GetOrAdd(type, _ => new SemaphoreSlim(1));

        await semaphore.WaitAsync(cancellationToken);

        try
        {
            channels.Add(channel);
        }
        finally
        {
            semaphore.Release();
        }
    }

    private async Task SafeRemoveChannelOfType<TEvent>(Channel<object> channel, CancellationToken cancellationToken) where TEvent : class
    {
        var type = typeof(TEvent);

        var channels = Channels.GetOrAdd(type, _ => new List<Channel<object>>());
        var semaphore = SemaphoreSlims.GetOrAdd(type, _ => new SemaphoreSlim(1));

        await semaphore.WaitAsync(cancellationToken);

        try
        {
            channels.Remove(channel);
        }
        finally
        {
            semaphore.Release();
        }
    }
}