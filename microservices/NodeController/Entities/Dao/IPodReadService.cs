using System.Collections.Immutable;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;

namespace Entities.Dao;

public interface IPodReadService
{
    IPodInstance? GetPodOfService(IServiceInstance serviceInstance);

    IPodInstance? GetPodById(string id);

    ImmutableList<IPodInstance> GetPodInstances(string? podType);

    ImmutableList<IPodInstance> GetAllPods();

    ImmutableList<IPodType> GetAllPodTypes();

    IPodType? GetPodType(string? podType);

    //----------------------------------------------------------------

    IServiceInstance? GetServiceById(string id);

    ImmutableList<IServiceInstance> GetServiceInstances(string? serviceType);

    ImmutableList<IServiceInstance> GetAllServices();

    ImmutableList<IServiceType> GetAllServiceTypes();

    IServiceType? GetServiceType(string serviceType);

    //----------------------------------------------------------------

    ImmutableList<int> GetTakenSocketPorts();

    int? TryGetSocketPortForType(IServiceType type);

    IServiceType? TryGetServiceTypeFromPort(int port);
}