using ApplicationLogic.Interfaces;
using Entities.DomainInterfaces.ResourceManagement;

namespace NodeController.External.Repository;

public class LogStoreWriteModel : ILogStoreWriteModel
{
    public async Task Log(ISnapshot snapshot)
    {
        throw new NotImplementedException();
    }
}