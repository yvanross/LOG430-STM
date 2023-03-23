using Entities.BusinessObjects.Live;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.ResourceManagement;

namespace Entities.BusinessObjects.States;

public class UnresponsiveState : IServiceState
{
    public void EvaluateState(IServiceInstance serviceInstance)
    {
        var deltaTime = DateTime.UtcNow.Subtract(serviceInstance.LastHeartbeat);

        if (deltaTime > TimeSpan.FromSeconds(5))
        {
            serviceInstance.ServiceStatus = new UnknownState();
        }
        if (deltaTime < TimeSpan.FromSeconds(2))
        {
            serviceInstance.ServiceStatus = new ReadyState();
        }
    }
}