using Entities.DomainInterfaces.Live;

namespace ApplicationLogic.Interfaces.Dao;

public interface IDataStreamReadModel
{
    void BeginStreaming(Action<ISaga> reportTestResult);

    void EndStreaming();
}