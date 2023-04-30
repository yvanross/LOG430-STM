using Entities.DomainInterfaces;

namespace Entities.BusinessObjects.States;

public class UnknownState : INodeState
{
    public void EvaluateState(INode node)
    {
        var deltaTime = DateTime.UtcNow.Subtract(node.LastSuccessfulPing);

        if (deltaTime < TimeSpan.FromSeconds(2))
        {
            node.ServiceStatus = new ReadyState();
        }
        if (deltaTime < TimeSpan.FromSeconds(5))
        {
            node.ServiceStatus = new UnresponsiveState();
        }
    }
}