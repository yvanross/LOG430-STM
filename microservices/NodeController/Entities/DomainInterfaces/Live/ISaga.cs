namespace Entities.DomainInterfaces.Live;

public interface ISaga
{
    int Phase { get; }

    int Seconds { get; }

    string Message { get; }
}