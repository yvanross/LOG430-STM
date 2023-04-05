using Entities.DomainInterfaces;

namespace ApplicationLogic.Interfaces;

public interface IDataStreamWriteModel
{
    Task Produce(IBusPositionUpdated busPositionUpdated);
}