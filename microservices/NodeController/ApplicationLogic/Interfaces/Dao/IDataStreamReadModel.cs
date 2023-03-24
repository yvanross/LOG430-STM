using Entities.DomainInterfaces.Live;

namespace ApplicationLogic.Interfaces.Dao;

public interface IDataStreamReadModel
{
    void BeginStreaming();

    Task EndStreaming();
}