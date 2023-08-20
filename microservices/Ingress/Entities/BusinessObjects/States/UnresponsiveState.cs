﻿using Entities.DomainInterfaces;

namespace Entities.BusinessObjects.States;

public class UnresponsiveState : INodeState
{
    public string GetStateName()
    {
        return "Unresponsive";
    }

    public void EvaluateState(INode node)
    {
        var deltaTime = DateTime.UtcNow.Subtract(node.LastSuccessfulPing);

        if (deltaTime > TimeSpan.FromSeconds(10))
        {
            node.ServiceStatus = new UnknownState();
        }
        if (deltaTime < TimeSpan.FromSeconds(2))
        {
            node.ServiceStatus = new ReadyState();
        }
    }
}