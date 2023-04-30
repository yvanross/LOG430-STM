namespace ApplicationLogic.Interfaces;

public interface IHostInfo
{
    string GetAddress();

    string GetPort();

    string GetMQServiceName();
}