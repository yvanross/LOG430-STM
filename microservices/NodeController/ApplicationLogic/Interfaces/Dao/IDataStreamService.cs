using Entities.DomainInterfaces.Live;

namespace ApplicationLogic.Interfaces.Dao;

public interface IDataStreamService
{
    Task Produce(ICoordinates coordinates);

    void EndStreaming();
}