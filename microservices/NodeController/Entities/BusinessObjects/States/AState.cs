using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.ResourceManagement;

namespace Entities.BusinessObjects.States;

public abstract class AState : IServiceState
{
    private protected readonly IServiceInstance ServiceInstance;

    protected internal AState(IServiceInstance serviceInstance)
    {
        ServiceInstance = serviceInstance;
    }

    public abstract void EvaluateState();
}