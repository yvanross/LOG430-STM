
// MassTransit URN type resolutions, namespaces must be equal between project for a shared type 
// ReSharper disable once CheckNamespace
namespace MqContracts;

public class BusPositionUpdated
{
    public double Seconds { get; set; }

    public string Message { get; set; }
}