using System.Collections.Concurrent;
using System.Reflection;
using Domain.Events.Interfaces;

namespace Domain.Events;

public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly ConcurrentDictionary<Type, MethodInfo> _handlerMethods = new();
    private readonly IServiceProvider _serviceProvider;

    public DomainEventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync(IDomainEvent domainEvent)
    {
        var eventType = domainEvent.GetType();

        var handleMethod = GetHandleMethod(eventType);

        await InvokeHandler(domainEvent, handleMethod);
    }

    private async Task InvokeHandler(IDomainEvent domainEvent, MethodBase handleMethod)
    {
        var handler = _serviceProvider.GetService(
            (handleMethod ??
             throw new InvalidOperationException(
                 $"No handler method found for the domain event type {domainEvent.GetType().Name}."))
            .DeclaringType ??
            throw new InvalidOperationException(
                $"Declaring type for the handle method not found for the domain event type {domainEvent.GetType().Name}."));

        if (handler == null) return;

        await ((Task)handleMethod.Invoke(handler, new object[] { domainEvent })!)!;
    }


    private MethodInfo GetHandleMethod(Type eventType)
    {
        if (_handlerMethods.TryGetValue(eventType, out var handleMethod)) return handleMethod;

        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

        var handlerMethods = handlerType.GetMethods().Where(m => m.Name == "HandleAsync").ToList();

        if (handlerMethods.Count != 1)
            throw new InvalidOperationException(
                $"Expected exactly one HandleAsync method on {handlerType.Name}, but found {handlerMethods.Count}.");

        handleMethod = handlerMethods.Single();

        _handlerMethods[eventType] = handleMethod;

        return handleMethod;
    }
}