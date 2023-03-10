using System.Collections.Concurrent;
using System.Collections.Immutable;
using Entities.DomainInterfaces;

namespace Ingress.Cache;

internal static class RouteCache
{
    //The portainer Address is the key
    private static ImmutableDictionary<string, ImmutableList<IService>> RegisteredServices { get; set; } = ImmutableDictionary<string, ImmutableList<IService>>.Empty;

    internal static ImmutableList<IService>? GetServices(string http)
    {
        return GetHostedServices(http);
    }

    internal static void AddOrUpdateService(IService service)
    {
        var services = GetHostedServices(service.Address);

        services = services.RemoveAll(TryMatchRoute);

        services = services.Add(service);

        SetHostedServices(service.Address, services);

        bool TryMatchRoute(IService registeredRoute)
         => registeredRoute.HttpRoute.Equals(service.HttpRoute) || registeredRoute.Id.Equals(service.Id);
    }

    private static void SetHostedServices(string http, ImmutableList<IService> services)
    {
        RegisteredServices = RegisteredServices.Remove(http);
        RegisteredServices = RegisteredServices.Add(http, services);
    }

    private static ImmutableList<IService>? GetHostedServices(string http)
    {
        RegisteredServices.TryGetValue(http, out var services);

        return services;
    }
}   