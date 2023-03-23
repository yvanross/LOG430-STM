﻿using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.ResourceManagement;

namespace Entities.BusinessObjects.States;

public class LaunchedState : IServiceState
{
    public void EvaluateState(IServiceInstance serviceInstance)
    {
        var deltaTime = DateTime.UtcNow.Subtract(serviceInstance.LastHeartbeat);

        if (deltaTime > TimeSpan.FromSeconds(8))
        {
            serviceInstance.ServiceStatus = new UnresponsiveState();
        }
        if(deltaTime < TimeSpan.FromSeconds(2))
        {
            serviceInstance.ServiceStatus = new ReadyState();
        }
    }
}