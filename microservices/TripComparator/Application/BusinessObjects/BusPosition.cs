using Application.Interfaces;

namespace Application.BusinessObjects;

public class BusPosition : IBusPositionUpdated
{
    public required int Seconds { get; init; }

    public required string Message { get; init; }
}