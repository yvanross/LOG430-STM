using System.Collections.Immutable;
using ApplicationLogic.Interfaces;
using Entities.DomainInterfaces;
using NodeController.Cache;

namespace NodeController.Repository;

public class RepositoryRead : IRepositoryRead
{
    private readonly string _http;

    public RepositoryRead(string http)
    {
        _http = http;
    }

    public IServiceInstance? ReadServiceById(Guid id)
    {
        return RouteCache.GetServices(_http)?.SingleOrDefault(route => route.Id.Equals(id));
    }

    public IServiceInstance? ReadServiceByAddressAndPort(string address, string port)
    {
        return RouteCache.GetServices(_http)?.SingleOrDefault(route => route.Address.Equals(address) && route.ContainerInfo.Port.Equals(port));
    }

    public ImmutableList<IServiceInstance>? ReadServiceByType(string serviceType)
    {
        return RouteCache.GetServices(_http)?.Where(route => route.Type.Equals(serviceType)).ToImmutableList();
    }

    public ImmutableList<IServiceInstance>? GetAllServices()
    {
        return RouteCache.GetServices(_http);
    }

    public IScheduler? GetScheduler()
    {
        return RouteCache.GetScheduler(_http);
    }

    public IServiceType? GetServiceType(string serviceType)
    {
        IServiceType containerConfig = default;

        RouteCache.GetContainerModels(_http)?.TryGetValue(serviceType, out containerConfig);

        return containerConfig;
    }

    public List<IServiceType>? GetAllServiceTypes()
    {
        return RouteCache.GetContainerModels(_http)?.Values.ToList();
    }

    public string GetAddress()
    {
        return _http;
    }
}