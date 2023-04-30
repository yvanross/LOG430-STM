using ApplicationLogic.Interfaces;

// MassTransit URN type resolutions, namespaces must be equal between project for a shared type 
// ReSharper disable once CheckNamespace

namespace MqContracts;

public class CoordinatesDto : ICoordinates
{
    public string StartingCoordinates { get; init; }

    public string DestinationCoordinates { get; init; }
}