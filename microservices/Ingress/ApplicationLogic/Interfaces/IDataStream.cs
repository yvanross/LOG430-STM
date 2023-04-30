
namespace ApplicationLogic.Interfaces.Dao;

public interface IDataStream
{
    Task Produce(string team, string message);
}