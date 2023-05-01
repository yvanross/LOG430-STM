﻿using ApplicationLogic.Interfaces.Dao;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;
using Infrastructure.Cache;

namespace Infrastructure.Dao;

public class PodWriteService : IPodWriteService
{
    public void AddOrUpdatePod(IPodInstance podInstance)
    {
        RouteCache.AddOrUpdatePodInstance(podInstance);
    }

    public void TryRemovePod(IPodInstance podInstance)
    {
        RouteCache.RemovePodInstance(podInstance);
    }

    public void AddOrUpdatePodType(IPodType podType)
    {
        RouteCache.AddOrUpdatePodType(podType);
    }
}