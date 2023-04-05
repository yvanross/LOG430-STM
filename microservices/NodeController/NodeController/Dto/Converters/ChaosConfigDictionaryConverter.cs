using Entities.DomainInterfaces.Planned;
using Entities.DomainInterfaces.ResourceManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using NodeController.Dto.Incoming;

namespace NodeController.Dto.Converters;

public class ChaosConfigDictionaryConverter : JsonConverter<ConcurrentDictionary<ArtifactTypeEnum, IChaosConfig>>
{
    public override void WriteJson(JsonWriter writer, ConcurrentDictionary<ArtifactTypeEnum, IChaosConfig>? value, JsonSerializer serializer)
    {
        var concurrentDict = value;

        serializer.Serialize(writer, concurrentDict.ToDictionary(kv => kv));
    }

    public override ConcurrentDictionary<ArtifactTypeEnum, IChaosConfig>? ReadJson(
        JsonReader reader,
        Type objectType,
        ConcurrentDictionary<ArtifactTypeEnum, IChaosConfig>? existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        var obj = JObject.Load(reader);

        var chaosConfigs = new ConcurrentDictionary<ArtifactTypeEnum, IChaosConfig>();
       
        foreach (var kvp in obj)
        {
            var key = kvp.Key.Trim('[', ']').Split(new string[] { ", " }, StringSplitOptions.TrimEntries)[0];

            var artifactType = Enum.Parse<ArtifactTypeEnum>(key);

            if ((kvp.Value as JObject)?.ToObject<ChaosConfigDto>(serializer) is { } chaosConfig)
                chaosConfigs.TryAdd(artifactType, chaosConfig);
        }

        return chaosConfigs;
    }
}