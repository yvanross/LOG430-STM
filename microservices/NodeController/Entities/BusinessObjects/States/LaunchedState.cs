using Entities.DomainInterfaces.ResourceManagement;

namespace Entities.BusinessObjects.States;

public class LaunchedState : IServiceState
{
    private readonly DateTime _stateChangeTime = DateTime.UtcNow;

    public DateTime GetStateChangeTime()
    {
        return _stateChangeTime;
    }

    public string GetStateName()
    {
        return "Started";
    }
}