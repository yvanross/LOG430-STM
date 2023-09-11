
// MassTransit URN type resolutions, namespaces must be equal between project for a shared type 
// ReSharper disable once CheckNamespace

namespace MqContracts;

public class CoordinateMessage
{
    public string StartingCoordinates { get; set; }

    public string DestinationCoordinates { get; set; }
}