using ApplicationLogic.Interfaces;
using Entities.DomainInterfaces.Live;

namespace NodeController.External.Repository;

public class DataStreamReadModel : IDataStreamReadModel
{


    public async Task<IEnumerable<ISaga>> GetData()
    {
        throw new NotImplementedException();
    }
}