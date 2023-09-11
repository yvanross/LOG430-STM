using Entities.DomainInterfaces.ResourceManagement;

namespace Entities.BusinessObjects.States;

public class ReadyState : IServiceState
{
    const int TimeToMaturity = 10;

    private readonly DateTime _stateChangeTime = DateTime.UtcNow;

    private readonly DateTime _maturedAt = DateTime.UtcNow.AddSeconds(TimeToMaturity);

    public bool IsMature => DateTime.UtcNow > _maturedAt;

    public DateTime GetStateChangeTime()
    {
        return _stateChangeTime;
    }

    public string GetStateName()
    {
        return "Ready";
    }
}