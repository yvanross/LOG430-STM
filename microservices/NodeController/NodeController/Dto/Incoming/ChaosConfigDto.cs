using Entities.DomainInterfaces.ResourceManagement;

namespace NodeController.Dto.Incoming;

public class ChaosConfigDto : IChaosConfig
{
    public long NanoCpus { get; set; }

    public long Memory { get; set; }

    public int MaxNumberOfPods { get; set; }

    public int KillRate { get; set; }
}