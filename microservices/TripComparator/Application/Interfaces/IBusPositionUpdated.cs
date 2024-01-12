namespace Application.Interfaces;

public interface IBusPositionUpdated
{
    double Seconds { get; }

    string Message { get; }
}