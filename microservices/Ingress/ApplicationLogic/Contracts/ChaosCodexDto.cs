// MassTransit URN type resolutions, namespaces must be equal between project for a shared type 
// ReSharper disable once CheckNamespace

namespace MqContracts;

public class ChaosCodexDto
{
    public Dictionary<string, ChaosConfigDto> ChaosConfigs { get; set; }

    public DateTime EndTestAt { get; set; }

    public DateTime StartTestAt { get; set; }
}