using System.Collections.Immutable;
using ApplicationLogic.Services;
using Entities.DomainInterfaces;

namespace NodeController.Cache;

internal static class RouteCache
{
    //The portainer Address is the key
    private static ImmutableDictionary<string, ImmutableList<IServiceInstance>> _servicesByPortainerAddress = ImmutableDictionary<string, ImmutableList<IServiceInstance>>.Empty;
    
    //The portainer Address is the key
    private static ImmutableDictionary<string, ImmutableDictionary<string, IServiceType>> _serviceTypeConfigByServiceTypeByPortainerAddress = ImmutableDictionary<string, ImmutableDictionary<string, IServiceType>>.Empty;

    //The portainer Address is the key
    private static ImmutableDictionary<string, IScheduler> _schedulerByPortainerAddress = ImmutableDictionary<string, IScheduler>.Empty;
    
    private static readonly object ServiceLock = new ();

    private static readonly object ServiceTypeLock = new ();

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
        var scheduler = GetRegisteredScheduler(http);

        if (scheduler is null)
        {
            ImmutableInterlocked.TryAdd(ref _schedulerByPortainerAddress, http, new Scheduler());

            scheduler = GetRegisteredScheduler(http);
        }

        return scheduler;
    }

    internal static void AddOrUpdateService(IServiceInstance serviceInstance)
    {
        lock (ServiceLock)
        {
            ImmutableInterlocked.AddOrUpdate(ref _servicesByPortainerAddress, serviceInstance.Address, _ => ImmutableList.Create(serviceInstance) , (_, list) =>
            {
                list = list.RemoveAll(TryMatchService);
                return list.Add(serviceInstance);
            });
        }

        bool TryMatchService(IServiceInstance registeredRoute)
         => registeredRoute.Id.Equals(serviceInstance.Id);
    }

    internal static void UpdateContainerModel(IServiceInstance serviceInstance, IServiceType containerConfig)
    {
        lock (ServiceTypeLock)
        {
            ImmutableInterlocked.AddOrUpdate(ref _serviceTypeConfigByServiceTypeByPortainerAddress, serviceInstance.Address, _ => ImmutableDictionary.Create<string, IServiceType>().Add(serviceInstance.Type, containerConfig), (_, dic) =>
            {
                dic = dic.Remove(serviceInstance.Type);
                return dic.Add(serviceInstance.Type, containerConfig);
            });
        }
    }

    internal static void RemoveService(IServiceInstance serviceInstance)
    {
        lock (ServiceLock)
        {
            var services = GetHostedServices(serviceInstance.Address);

            if (services is null) return;

            services = services.RemoveAll(TryMatchService);

            SetHostedServices(serviceInstance.Address, services);

            bool TryMatchService(IServiceInstance registeredRoute)
                => registeredRoute.Id.Equals(serviceInstance.Id);
        }
    }

    private static void SetHostedServices(string http, ImmutableList<IServiceInstance> services)
    {
        _servicesByPortainerAddress = _servicesByPortainerAddress.Remove(http);
        _servicesByPortainerAddress = _servicesByPortainerAddress.Add(http, services);
    }

    private static ImmutableList<IServiceInstance>? GetHostedServices(string http)
    {
        _servicesByPortainerAddress.TryGetValue(http, out var services);

        return services;
    }

    private static ImmutableDictionary<string, IServiceType>? GetRegisteredContainerModels(string http)
    {
        _serviceTypeConfigByServiceTypeByPortainerAddress.TryGetValue(http, out var services);

        return services;
    }

    private static IScheduler? GetRegisteredScheduler(string http)
    {
        _schedulerByPortainerAddress.TryGetValue(http, out var scheduler);

        return scheduler;
    }
}   