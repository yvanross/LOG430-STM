using Entities.DomainInterfaces.ResourceManagement;

namespace ApplicationLogic.Interfaces.Dao;

public interface ISystemStateWriteService
{
    Task Log(IExperimentReport experimentReport);
}