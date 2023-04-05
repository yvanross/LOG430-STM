using System.Collections.Concurrent;
using Entities.DomainInterfaces.Planned;
using Entities.DomainInterfaces.ResourceManagement;
using Newtonsoft.Json;
using NodeController.Dto.Converters;

namespace NodeController.Dto.Incoming;

public class ChaosCodexDto : IChaosCodex
{
    [JsonConverter(typeof(ChaosConfigDictionaryConverter))]
    public ConcurrentDictionary<ArtifactTypeEnum, IChaosConfig> ChaosConfigs { get; set; }

    public int AcceptableAverageLatencyInMs { get; set; }

    public DateTime EndTestAt { get; set; }

    public DateTime StartTestAt { get; set; }
}