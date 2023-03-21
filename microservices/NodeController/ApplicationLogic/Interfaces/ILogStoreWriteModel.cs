using Entities.DomainInterfaces.ResourceManagement;

namespace ApplicationLogic.Interfaces;

public interface ILogStoreWriteModel
{
    Task Log(ISnapshot snapshot);
}