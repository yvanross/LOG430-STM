namespace ApplicationLogic.Interfaces;

public interface IHostInfo
{
    string GetTeamName();

    string GetIngressAddress();

    string GetIngressPort();

    string GetAddress();

    string GetPort();
}