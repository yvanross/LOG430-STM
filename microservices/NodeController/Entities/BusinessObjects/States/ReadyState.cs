using Entities.DomainInterfaces.ResourceManagement;

namespace Entities.BusinessObjects.States;

public class ReadyState : IServiceState
{
    const int TimeToMaturity = 10;

    private DateTime _maturedAt = DateTime.UtcNow.AddSeconds(TimeToMaturity);

    public bool IsMature => DateTime.UtcNow > _maturedAt;

    public string GetStateName()
    {
        return "Ready";
    }
}