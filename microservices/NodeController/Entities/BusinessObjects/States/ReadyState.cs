using Entities.DomainInterfaces;

namespace Entities.BusinessObjects.States;

public class ReadyState : AState
{
    public ReadyState(IServiceInstance serviceInstance) : base(serviceInstance) { }

    public override void EvaluateState()
    {
        var deltaTime = DateTime.UtcNow.Subtract(ServiceInstance.LastHeartbeat);

        if (deltaTime > TimeSpan.FromSeconds(2))
        {
            ServiceInstance.ServiceStatus = new UnresponsiveState(ServiceInstance);
        }
    }
}