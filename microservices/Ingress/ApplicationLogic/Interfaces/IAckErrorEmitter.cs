
using MqContracts;

namespace ApplicationLogic.Interfaces.Dao;

public interface IAckErrorEmitter<T>
{
    Task Produce(string routingKey, T ackError);
}