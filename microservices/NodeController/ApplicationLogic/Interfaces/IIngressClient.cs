namespace ApplicationLogic.Interfaces;

public interface IIngressClient
{
    public Task Subscribe();

    public Task SendUpdate();
}