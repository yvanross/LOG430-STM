namespace ApplicationLogic.Interfaces.Dao;

public interface IDataStreamService
{
    Task Produce(ICoordinates coordinates);
}