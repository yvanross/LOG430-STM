using Entities.BusinessObjects.Live;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;
using Entities.DomainInterfaces.ResourceManagement;

namespace ApplicationLogic.Interfaces;

public interface IRouting
{
    IEnumerable<RoutingData> RouteByDestinationType(string sourceServiceId, string destinationServiceType, LoadBalancingMode mode);

    List<IServiceInstance> LoadBalancing(List<IServiceInstance> services, LoadBalancingMode mode);

    void RegisterUnresponsive(IServiceInstance serviceInstance);

    int NegotiateSocket(IServiceType type);
}