using Entities.DomainInterfaces.Live;

namespace Entities.DomainInterfaces.ResourceManagement;

public interface IServiceState
{
    public string GetStateName();

    public void EvaluateState(IServiceInstance serviceInstance);
}