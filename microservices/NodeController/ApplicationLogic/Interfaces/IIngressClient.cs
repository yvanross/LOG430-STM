namespace ApplicationLogic.Interfaces;

public interface IIngressClient
{
    public Task Subscribe(string teamName, string address, string port);

    public string GetLogStoreAddressAndPort();
}