namespace Entities.DomainInterfaces.Live;

public interface ISaga
{
    int Seconds { get; }

    string Message { get; }
}