using System.Collections.Immutable;
using Entities.DomainInterfaces;

namespace ApplicationLogic.Interfaces;

public interface IRepositoryRead
{
    public IService? ReadServiceById(Guid name);

    public IService? ReadServiceByAddressAndPort(string address, string port);

    public ImmutableList<IService>? ReadServiceByType(string serviceType);

    public ImmutableList<IService>? GetAllServices();
}