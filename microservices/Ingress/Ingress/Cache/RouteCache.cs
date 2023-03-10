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
    private static ImmutableDictionary<string, ImmutableList<IService>> ServicesByPortainerAddress { get; set; } = ImmutableDictionary<string, ImmutableList<IService>>.Empty;
    
    //The portainer Address is the key
    private static ImmutableDictionary<string, ImmutableDictionary<string, IContainerConfigName>> ContainerConfigByServiceTypeByPortainerAddress { get; set; } = ImmutableDictionary<string, ImmutableDictionary<string, IContainerConfigName>>.Empty;

    internal static ImmutableList<IService>? GetServices(string http)
    {
        return GetHostedServices(http);
    }

    internal static ImmutableDictionary<string, IContainerConfigName>? GetContainerModels(string http)
    {
        return GetRegisteredContainerModels(http);
    }

    internal static void AddOrUpdateService(IService service)
    {
        var services = GetHostedServices(service.Address);
        
        if (services is null) return;

        services = services.RemoveAll(TryMatchService);

        services = services.Add(service);

        SetHostedServices(service.Address, services);

        bool TryMatchService(IService registeredRoute)
         => registeredRoute.HttpRoute.Equals(service.HttpRoute) || registeredRoute.Id.Equals(service.Id);
    }

    internal static void UpdateContainerModel(IService service, IContainerConfigName containerConfig)
    {
        var containerModels = GetRegisteredContainerModels(service.Address);

        if (containerModels is null) return;

        containerModels = containerModels.Remove(service.ServiceType);

        containerModels = containerModels.Add(service.ServiceType, containerConfig);

        SetRegisteredContainerModel(service.Address, containerModels);
    }

    internal static void RemoveService(IService service)
    {
        var services = GetHostedServices(service.Address);

        if (services is null) return;

        services = services.RemoveAll(TryMatchService);

        SetHostedServices(service.Address, services);

        bool TryMatchService(IService registeredRoute)
            => registeredRoute.HttpRoute.Equals(service.HttpRoute) || registeredRoute.Id.Equals(service.Id);
    }

    private static void SetHostedServices(string http, ImmutableList<IService> services)
    {
        ServicesByPortainerAddress = ServicesByPortainerAddress.Remove(http);
        ServicesByPortainerAddress = ServicesByPortainerAddress.Add(http, services);
    }

    private static ImmutableList<IService>? GetHostedServices(string http)
    {
        ServicesByPortainerAddress.TryGetValue(http, out var services);

        return services;
    }

    private static void SetRegisteredContainerModel(string http, ImmutableDictionary<string, IContainerConfigName> containerModels)
    {
        ContainerConfigByServiceTypeByPortainerAddress = ContainerConfigByServiceTypeByPortainerAddress.Remove(http);
        ContainerConfigByServiceTypeByPortainerAddress = ContainerConfigByServiceTypeByPortainerAddress.Add(http, containerModels);
    }

    private static ImmutableDictionary<string, IContainerConfigName>? GetRegisteredContainerModels(string http)
    {
        ContainerConfigByServiceTypeByPortainerAddress.TryGetValue(http, out var services);

        return services;
    }
}   