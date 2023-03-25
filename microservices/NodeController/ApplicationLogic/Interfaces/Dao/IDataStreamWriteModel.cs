﻿using Entities.DomainInterfaces.ResourceManagement;

namespace ApplicationLogic.Interfaces.Dao;

public interface IDataStreamWriteModel
{
    Task Log(IExperimentReport experimentReport);
}