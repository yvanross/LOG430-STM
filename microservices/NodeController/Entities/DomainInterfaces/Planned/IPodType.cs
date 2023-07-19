using System.Collections.Immutable;

namespace Entities.DomainInterfaces.Planned;

public interface IPodType
{
    string Type { get; set; }

    bool ShareVolumes { get; set; }

    int NumberOfInstances { get; }

    ImmutableList<string> ReplicasHostnames { get; }

    ImmutableList<IServiceType> ServiceTypes { get; }

    IServiceType? PodLeader { get; }

    void IncreaseNumberOfPod();

    void DecreaseNumberOfPod();

    void SetNumberOfPod(int numberOfInstances);

    void AddHostname(string hostname);

    void AddRangeHostnames(IEnumerable<string> hostnames);

    void AddServiceTypes(string serviceType);

    void AddRangeServiceTypes(IEnumerable<string> serviceTypes);

    void SetPodLeader(string podLeader);
}
