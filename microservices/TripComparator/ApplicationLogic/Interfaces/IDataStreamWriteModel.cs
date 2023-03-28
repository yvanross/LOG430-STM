using Entities.DomainInterfaces;

namespace ApplicationLogic.Interfaces;

public interface IDataStreamWriteModel
{
    Task BeginStreaming();

    void CloseChannel();

    Task Produce(ISaga saga);
}