using ApplicationLogic.Interfaces;

namespace NodeController.Dto.Incoming;

public class CoordinatesDto : ICoordinates
{
    public string StartingCoordinates { get; init; }

    public string DestinationCoordinates { get; init; }
}