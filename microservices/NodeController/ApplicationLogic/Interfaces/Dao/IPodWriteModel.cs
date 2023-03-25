﻿using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;

namespace ApplicationLogic.Interfaces.Dao;

public interface IPodWriteModel
{
    void AddOrUpdatePod(IPodInstance podInstance);

    void TryRemovePod(IPodInstance podInstance);

    void AddOrUpdatePodType(IPodType podType);

}