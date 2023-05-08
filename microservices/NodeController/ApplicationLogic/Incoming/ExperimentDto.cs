

// MassTransit URN type resolutions, namespaces must be equal between project for a shared type 
// ReSharper disable once CheckNamespace

namespace MqContracts;

public class ExperimentDto
{
    public ChaosCodexDto ChaosCodex { get; set; }

    public CoordinatesDto Coordinates { get; set; }
}