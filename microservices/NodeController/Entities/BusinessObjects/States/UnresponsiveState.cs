using Entities.DomainInterfaces.Live;

namespace Entities.BusinessObjects.States;

public class UnresponsiveState : AState
{
    public UnresponsiveState(IServiceInstance serviceInstance) : base(serviceInstance) { }

    public override void EvaluateState()
    {
        var deltaTime = DateTime.UtcNow.Subtract(ServiceInstance.LastHeartbeat);

        if (deltaTime > TimeSpan.FromSeconds(5))
        {
            ServiceInstance.ServiceStatus = new UnknownState(ServiceInstance);
        }
        if (deltaTime < TimeSpan.FromSeconds(2))
        {
            ServiceInstance.ServiceStatus = new ReadyState(ServiceInstance);
        }
    }
}