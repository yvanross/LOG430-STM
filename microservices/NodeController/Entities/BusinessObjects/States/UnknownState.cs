using Entities.DomainInterfaces.Live;

namespace Entities.BusinessObjects.States;

public class UnknownState : AState
{
    public UnknownState(IServiceInstance serviceInstance) : base(serviceInstance) { }

    public override void EvaluateState()
    {
        var deltaTime = DateTime.UtcNow.Subtract(ServiceInstance.LastHeartbeat);

        if (deltaTime < TimeSpan.FromSeconds(2))
        {
            ServiceInstance.ServiceStatus = new ReadyState(ServiceInstance);
        }
        if (deltaTime < TimeSpan.FromSeconds(5))
        {
            ServiceInstance.ServiceStatus = new UnresponsiveState(ServiceInstance);
        }
    }
}