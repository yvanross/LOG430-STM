using System.Collections.Concurrent;
using Entities.DomainInterfaces.Planned;

namespace Entities.DomainInterfaces.ResourceManagement;

public interface IChaosCodex
{
    ConcurrentDictionary<ArtifactTypeEnum, IChaosConfig> ChaosConfigs { get; }

    DateTime EndTestAt { get; }

    DateTime StartTestAt { get; }
}