namespace ApplicationLogic.Interfaces;

public interface IIngressClient
{
    Task Subscribe(string group, string teamName, string username, string secret, string address, string port);

    public Task<string> GetLogStoreAddressAndPort(string teamName);
}