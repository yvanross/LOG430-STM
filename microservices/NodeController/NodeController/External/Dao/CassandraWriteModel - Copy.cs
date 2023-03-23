using ApplicationLogic.Interfaces.Dao;
using Entities.DomainInterfaces.ResourceManagement;

namespace NodeController.External.Repository;

public class SystemStateStorageWriteModel : ISystemStateStorageWriteModel
{
    public async Task Log(ISnapshot snapshot)
    {
        throw new NotImplementedException();
    }
}