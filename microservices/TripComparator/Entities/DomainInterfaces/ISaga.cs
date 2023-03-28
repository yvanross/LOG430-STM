namespace Entities.DomainInterfaces;

public interface ISaga
{
    int Seconds { get; }

    string Message { get; }
}