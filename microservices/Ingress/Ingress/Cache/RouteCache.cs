using System.Collections.Concurrent;
using System.Collections.Immutable;
using Entities.DomainInterfaces;

namespace Ingress.Cache;

internal static class RouteCache
{
    internal static ImmutableList<IRoute> RegisteredRoutes { get; private set; } = ImmutableList<IRoute>.Empty;

    internal static void AddRoute(IRoute route)
    {
        RegisteredRoutes = RegisteredRoutes.RemoveAll(TryMatchRoute);

        RegisteredRoutes = RegisteredRoutes.Add(route);

        bool TryMatchRoute(IRoute registeredRoute)
         => registeredRoute.HttpRoute.Equals(route.HttpRoute);
    }
}   