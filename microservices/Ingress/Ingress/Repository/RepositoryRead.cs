using System.Collections.Immutable;
using ApplicationLogic.Interfaces;
using Entities.BusinessObjects;
using Entities.DomainInterfaces;
using Ingress.Cache;
using Ingress.Interfaces;

namespace Ingress.Repository;

public class RepositoryRead : IRepositoryRead
{
    private readonly string _http;

    public RepositoryRead(string http)
    {
        _http = http;
    }

    public IService? ReadServiceById(Guid id)
    {
        return RouteCache.GetServices(_http)?.SingleOrDefault(route => route.Id.Equals(id));
    }

    public IService? ReadServiceByAddressAndPort(string address, string port)
    {
        return RouteCache.GetServices(_http)?.SingleOrDefault(route => route.Address.Equals(address) && route.ContainerInfo.Port.Equals(port));
    }

    public ImmutableList<IService>? ReadServiceByType(string serviceType)
    {
        return RouteCache.GetServices(_http)?.Where(route => route.ServiceType.Equals(serviceType)).ToImmutableList();
    }

    public ImmutableList<IService>? GetAllServices()
    {
        return RouteCache.GetServices(_http);
    }

    public IContainerConfigName? GetContainerModel(string serviceType)
    {
        IContainerConfigName? containerConfig = default;

        RouteCache.GetContainerModels(_http)?.TryGetValue(serviceType, out containerConfig);

        return containerConfig;
    }
}