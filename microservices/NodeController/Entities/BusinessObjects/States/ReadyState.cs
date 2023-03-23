using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.ResourceManagement;

namespace Entities.BusinessObjects.States;

public class ReadyState : IServiceState
{
    public void EvaluateState(IServiceInstance serviceInstance)
    {
        var deltaTime = DateTime.UtcNow.Subtract(serviceInstance.LastHeartbeat);

        if (deltaTime > TimeSpan.FromSeconds(2))
        {
            serviceInstance.ServiceStatus = new UnresponsiveState();
        }
    }
}