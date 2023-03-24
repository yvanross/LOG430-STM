using Entities.DomainInterfaces.ResourceManagement;

namespace ApplicationLogic.Interfaces.Dao;

public interface ISystemStateWriteModel
{
    Task Log(IExperimentReport experimentReport);
}