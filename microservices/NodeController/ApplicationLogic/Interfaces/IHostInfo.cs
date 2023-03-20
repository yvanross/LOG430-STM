namespace ApplicationLogic.Interfaces;

public interface IHostInfo
{
    string GetPort();

    string GetIngressAddress();
}