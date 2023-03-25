using Entities.DomainInterfaces.Live;

namespace ApplicationLogic.Interfaces.Dao;

public interface IDataStreamReadModel
{
    void BeginStreaming(Func<ISaga, Task> reportTestResult);

    Task EndStreaming();
}