namespace ApplicationLogic.Interfaces;

public interface ICallBackClient
{
    string Key { get; }

    public Task CallBack(string message, string deltaTime, bool closingConnection);
}