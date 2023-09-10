using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;

namespace Entities.Dao;

public interface IPodWriteService
{
    void AddOrUpdatePod(IPodInstance podInstance);

    void TryRemovePod(IPodInstance podInstance);

    void AddOrUpdateServiceType(IServiceType serviceType);

    void AddOrUpdatePodType(IPodType podType);

    void RemovePodType(IPodType podType);

    void AddTunnel(int port, IServiceType type);
}