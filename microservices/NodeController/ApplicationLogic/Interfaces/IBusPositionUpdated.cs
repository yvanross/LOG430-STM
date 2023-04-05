namespace ApplicationLogic.Interfaces;

public interface IBusPositionUpdated
{
    int Seconds { get; }

    string Message { get; }
}