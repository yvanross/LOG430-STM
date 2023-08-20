﻿using Entities.DomainInterfaces;

namespace Entities.BusinessObjects.States;

public class ReadyState : INodeState
{
    public string GetStateName()
    {
        return "Ready";
    }

    public void EvaluateState(INode node)
    {
        var deltaTime = DateTime.UtcNow.Subtract(node.LastSuccessfulPing);

        if (deltaTime > TimeSpan.FromSeconds(2))
        {
            node.ServiceStatus = new UnresponsiveState();
        }
    }
}