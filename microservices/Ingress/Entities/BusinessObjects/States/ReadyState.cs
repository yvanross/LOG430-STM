using Entities.DomainInterfaces;
using Entities.DomainInterfaces.ResourceManagement;

namespace Entities.BusinessObjects.States;

public class ReadyState : INodeState
{
    public void EvaluateState(INode node)
    {
        var deltaTime = DateTime.UtcNow.Subtract(node.LastSuccessfulPing);

        if (deltaTime > TimeSpan.FromSeconds(2))
        {
            node.ServiceStatus = new UnresponsiveState();
        }
    }
}