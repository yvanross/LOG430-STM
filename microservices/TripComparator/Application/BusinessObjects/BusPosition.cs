using Application.Interfaces;

namespace Application.BusinessObjects;

public class BusPosition : IBusPositionUpdated
{
    public required double Seconds { get; init; }

    public required string Message { get; init; }
}