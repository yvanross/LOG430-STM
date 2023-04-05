namespace Entities.DomainInterfaces;

public interface IBusPositionUpdated
{
    int Seconds { get; }

    string Message { get; }
}