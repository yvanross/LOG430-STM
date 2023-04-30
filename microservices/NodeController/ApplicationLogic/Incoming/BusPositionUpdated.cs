using ApplicationLogic.Interfaces;

// MassTransit URN type resolutions, namespaces must be equal between project for a shared type 
// ReSharper disable once CheckNamespace
namespace MqContracts;

public class BusPositionUpdated : IBusPositionUpdated
{
    public int Seconds { get; set; }

    public string Message { get; set; }
}