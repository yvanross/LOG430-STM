using System.Collections.Immutable;
using Entities.BusinessObjects;
using Entities.DomainInterfaces;

namespace ApplicationLogic.Interfaces;

public interface IRepositoryRead
{
    IServiceInstance? ReadServiceById(Guid name);

    IServiceInstance? ReadServiceByAddressAndPort(string address, string port);

    ImmutableList<IServiceInstance>? ReadServiceByType(string serviceType);

    ImmutableList<IServiceInstance>? GetAllServices();

    IScheduler? GetScheduler();

    IServiceType? GetServiceType(string serviceType);
   
    List<IServiceType>? GetAllServiceTypes();
    
    string GetAddress();
}