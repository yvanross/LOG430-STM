using System.Collections.Concurrent;
using ApplicationLogic.Converters;
using Entities.DomainInterfaces.Planned;
using Entities.DomainInterfaces.ResourceManagement;
using Newtonsoft.Json;

// MassTransit URN type resolutions, namespaces must be equal between project for a shared type 
// ReSharper disable once CheckNamespace

namespace MqContracts;

public class ChaosCodexDto : IChaosCodex
{
    [JsonConverter(typeof(ChaosConfigDictionaryConverter))]
    public ConcurrentDictionary<ArtifactTypeEnum, IChaosConfig> ChaosConfigs { get; set; }

    public int AcceptableAverageLatencyInMs { get; set; }

    public DateTime EndTestAt { get; set; }

    public DateTime StartTestAt { get; set; }
}