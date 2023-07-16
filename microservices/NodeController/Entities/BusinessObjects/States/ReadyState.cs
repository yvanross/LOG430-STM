using Entities.DomainInterfaces.ResourceManagement;

namespace Entities.BusinessObjects.States;

public class ReadyState : IServiceState
{
    public string GetStateName()
    {
        return "Ready";
    }
}