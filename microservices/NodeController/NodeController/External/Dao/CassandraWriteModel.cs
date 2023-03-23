using ApplicationLogic.Interfaces.Dao;
using Entities.DomainInterfaces.ResourceManagement;

namespace NodeController.External.Repository;

public class RedisWriteModel : ISystemStateStorageWriteModel
{
    public async Task Log(ISnapshot snapshot)
    {
        throw new NotImplementedException();
    }
}