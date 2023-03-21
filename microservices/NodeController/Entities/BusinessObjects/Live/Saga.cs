using Entities.DomainInterfaces.Live;

namespace Entities.BusinessObjects.Live;

public class Saga : ISaga
{
    public required int Phase { get; init;}

    public required int Seconds { get; init; }

    public required string Message { get; init; }
}