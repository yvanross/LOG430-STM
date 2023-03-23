using Entities.DomainInterfaces.Live;

namespace ApplicationLogic.Interfaces.Dao;

public interface IDataStreamReadModel
{
    public void BeginStreaming();
}