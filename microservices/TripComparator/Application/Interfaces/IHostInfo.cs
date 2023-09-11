namespace Application.Interfaces;

public interface IHostInfo
{
    string GetAddress();

    string GetPort();

    string GetMQServiceName();
}