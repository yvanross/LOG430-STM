using Entities.DomainInterfaces.ResourceManagement;

namespace ApplicationLogic.Interfaces.Dao;

public interface ISystemStateStorageWriteModel
{
    Task Log(ISnapshot snapshot);
}