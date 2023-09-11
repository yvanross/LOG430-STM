namespace Application.Interfaces;

public interface IDataStreamWriteModel
{
    Task Produce(IBusPositionUpdated busPositionUpdated);
}