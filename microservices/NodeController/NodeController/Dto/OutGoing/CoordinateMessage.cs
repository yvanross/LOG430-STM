
// MassTransit URN type resolutions, namespaces must be equal between project for a shared type 
// ReSharper disable once CheckNamespace

namespace MqContracts;

public record CoordinateMessage
{
    public string StartingCoordinates { get; init; }

    public string DestinationCoordinates { get; init; }
}