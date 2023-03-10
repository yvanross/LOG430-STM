using System.Collections.Immutable;
using Entities.BusinessObjects;
using Entities.DomainInterfaces;
using Ingress.Interfaces;

namespace ApplicationLogic.Interfaces;

public interface IRepositoryRead
{
    IService? ReadServiceById(Guid name);

    IService? ReadServiceByAddressAndPort(string address, string port);

    ImmutableList<IService>? ReadServiceByType(string serviceType);

    ImmutableList<IService>? GetAllServices();

    IContainerConfigName? GetContainerModel(string serviceType);
}