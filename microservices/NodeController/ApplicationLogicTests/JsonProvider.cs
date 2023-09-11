using System.Collections.Concurrent;
using Entities.DomainInterfaces.Planned;
using Entities.DomainInterfaces.ResourceManagement;
using MqContracts;
using Newtonsoft.Json;

namespace ApplicationLogicTests;

public class JsonProvider
{
    public static string GenerateExperimentBody()
    {
        var experiment = new ExperimentDto
        {
            ChaosCodex = new ChaosCodexDto()
            {
                ChaosConfigs = new ConcurrentDictionary<ArtifactTypeEnum, IChaosConfig>
                {
                    [ArtifactTypeEnum.Computation] = new ChaosConfigDto()
                    {
                        KillRate = 0,
                        MaxNumberOfPods = 100,
                        Memory = 0,
                        NanoCpus = 0
                    },
                    [ArtifactTypeEnum.Connector] = new ChaosConfigDto()
                    {
                        KillRate = 0,
                        MaxNumberOfPods = 100,
                        Memory = 0,
                        NanoCpus = 0
                    },
                    [ArtifactTypeEnum.Database] = new ChaosConfigDto()
                    {
                        KillRate = 0,
                        MaxNumberOfPods = 100,
                        Memory = 0,
                        NanoCpus = 0
                    }
                },
                Duration = DateTime.UtcNow.AddDays(100),
            },

            Coordinates = new CoordinatesDto()
            {
                StartingCoordinates = "45.50827613912489, -73.57132606786716",
                DestinationCoordinates = "45.49648537116182, -73.58175449661363"
            }
        };

        return JsonConvert.SerializeObject(experiment);
    }
}