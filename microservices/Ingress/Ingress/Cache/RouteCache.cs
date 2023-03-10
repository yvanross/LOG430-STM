using System.Collections.Concurrent;
using System.Collections.Immutable;
using Ambassador;
using Entities.BusinessObjects;
using Entities.DomainInterfaces;
using Ingress.Interfaces;

namespace Ingress.Cache;

internal static class RouteCache
{
    //The portainer Address is the key
    private static ImmutableDictionary<string, ImmutableList<IServiceInstance>> ServicesByPortainerAddress { get; set; } = ImmutableDictionary<string, ImmutableList<IServiceInstance>>.Empty;
    
    //The portainer Address is the key
    private static ImmutableDictionary<string, ImmutableDictionary<string, IServiceType>> ServiceTypeConfigByServiceTypeByPortainerAddress { get; set; } = ImmutableDictionary<string, ImmutableDictionary<string, IServiceType>>.Empty;

    //The portainer Address is the key
    private static ImmutableDictionary<string, IScheduler> SchedulerByPortainerAddress { get; set; } = ImmutableDictionary<string, IScheduler>.Empty;

    internal static ImmutableList<IServiceInstance>? GetServices(string http)
    {
        return GetHostedServices(http);
    }

    internal static ImmutableDictionary<string, IServiceType>? GetContainerModels(string http)
    {
        return GetRegisteredContainerModels(http);
    }

    internal static IScheduler? GetScheduler(string http)
    {
        return GetRegisteredScheduler(http);
    }

    internal static void AddOrUpdateService(IServiceInstance serviceInstance)
    {
        var services = GetHostedServices(serviceInstance.Address);

        if (services is null)
        {
            services = ImmutableList<IServiceInstance>.Empty;

            ServicesByPortainerAddress = ServicesByPortainerAddress.Add(serviceInstance.Address, services);
            SchedulerByPortainerAddress = SchedulerByPortainerAddress.Add(serviceInstance.Address, new Scheduler());
        }

        services = services.RemoveAll(TryMatchService);

        services = services.Add(serviceInstance);

        SetHostedServices(serviceInstance.Address, services);

        bool TryMatchService(IServiceInstance registeredRoute)
         => registeredRoute.HttpRoute.Equals(serviceInstance.HttpRoute) || registeredRoute.Id.Equals(serviceInstance.Id);
    }

    internal static void UpdateContainerModel(IServiceInstance serviceInstance, IServiceType containerConfig)
    {
        var containerModels = GetRegisteredContainerModels(serviceInstance.Address);

        if (containerModels is null)
        {
            containerModels = ImmutableDictionary<string, IServiceType>.Empty;

            ServiceTypeConfigByServiceTypeByPortainerAddress = ServiceTypeConfigByServiceTypeByPortainerAddress.Add(serviceInstance.Address, containerModels);
        }

        containerModels = containerModels.Remove(serviceInstance.Type);

        containerModels = containerModels.Add(serviceInstance.Type, containerConfig);

        SetRegisteredContainerModel(serviceInstance.Address, containerModels);
    }

    internal static void RemoveService(IServiceInstance serviceInstance)
    {
        var services = GetHostedServices(serviceInstance.Address);

        if (services is null) return;

        services = services.RemoveAll(TryMatchService);

        SetHostedServices(serviceInstance.Address, services);

        bool TryMatchService(IServiceInstance registeredRoute)
            => registeredRoute.HttpRoute.Equals(serviceInstance.HttpRoute) || registeredRoute.Id.Equals(serviceInstance.Id);
    }

    private static void SetHostedServices(string http, ImmutableList<IServiceInstance> services)
    {
        ServicesByPortainerAddress = ServicesByPortainerAddress.Remove(http);
        ServicesByPortainerAddress = ServicesByPortainerAddress.Add(http, services);
    }

    private static ImmutableList<IServiceInstance>? GetHostedServices(string http)
    {
        ServicesByPortainerAddress.TryGetValue(http, out var services);

        return services;
    }

    private static void SetRegisteredContainerModel(string http, ImmutableDictionary<string, IServiceType> containerModels)
    {
        ServiceTypeConfigByServiceTypeByPortainerAddress = ServiceTypeConfigByServiceTypeByPortainerAddress.Remove(http);
        ServiceTypeConfigByServiceTypeByPortainerAddress = ServiceTypeConfigByServiceTypeByPortainerAddress.Add(http, containerModels);
    }

    private static ImmutableDictionary<string, IServiceType>? GetRegisteredContainerModels(string http)
    {
        ServiceTypeConfigByServiceTypeByPortainerAddress.TryGetValue(http, out var services);

        return services;
    }

    private static IScheduler? GetRegisteredScheduler(string http)
    {
        SchedulerByPortainerAddress.TryGetValue(http, out var scheduler);

        return scheduler;
    }
}   