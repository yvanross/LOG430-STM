using Entities.DomainInterfaces.ResourceManagement;

// MassTransit URN type resolutions, namespaces must be equal between project for a shared type 
// ReSharper disable once CheckNamespace

namespace MqContracts;

public class ChaosConfigDto : IChaosConfig
{
    public long NanoCpus { get; set; }

    public long Memory { get; set; }

    public int MaxNumberOfPods { get; set; }

    public int KillRate { get; set; }
}