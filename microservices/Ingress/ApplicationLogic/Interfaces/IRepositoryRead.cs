using Entities.DomainInterfaces;

namespace ApplicationLogic.Interfaces;

public interface IRepositoryRead
{
    public IRoute? ReadRouteById(string name);

    public IRoute? ReadRouteByAddressAndPort(string address, string port);

    public IRoute? ReadRouteByType(string serviceType);
}