using Entities.DomainInterfaces;

namespace TripComparator.DTO;

public class Saga : ISaga
{
    public required int Seconds { get; init; }

    public required string Message { get; init; }
}