using MqContracts;

namespace ApplicationLogic.Interfaces.Dao;

public interface IHeartbeatService
{
    Task Produce(HeartBeatDto heartBeatDto);
}