using Entities.BusinessObjects.Live;
using Entities.DomainInterfaces.Live;
using Entities.DomainInterfaces.Planned;
using System.Collections.Concurrent;

namespace MqContracts;

public class ServiceConfigurationWrapper
{
    public string ArtifactName { get; }

    public string ArtifactCategory { get; }

    public int NumberOfInstancesAsNumber { get; }

    public IEnumerable<string> NumberOfInstancesAsHostNames { get; }

    public string PodName { get; }

    public string? PodId { get; }

    public string Dns { get; }

    public string ServiceId { get; }

    public IEnumerable<string> PodLinks { get; }

    public bool ShareVolumesWithReplicas { get; }

    public IContainerConfig RawConfig { get; }

    public ContainerInfo CuratedInfo { get; }

    public ServiceConfigurationWrapper(ContainerInfo curatedInfo, IContainerConfig rawConfig)
    {
        ArtifactName = GetArtifactName(curatedInfo, rawConfig);
        ArtifactCategory = GetArtifactCategory(curatedInfo);
        NumberOfInstancesAsNumber = GetNumberOfInstancesAsNumber(curatedInfo);
        NumberOfInstancesAsHostNames = GetNumberOfInstancesAsHostNames(curatedInfo);
        PodName = GetPodName(curatedInfo, rawConfig);
        PodId = GetPodId(curatedInfo);
        Dns = GetDnsAccessibilityModifier(curatedInfo);
        PodLinks = GetPodLinks(curatedInfo);
        ShareVolumesWithReplicas = GetShareVolumesWithReplicas(curatedInfo);
        ServiceId = GetArtifactNameFromEnvironmentId(curatedInfo, rawConfig);

        CuratedInfo = curatedInfo;
        RawConfig = rawConfig;

        CheckForRoutingPort(CuratedInfo);
    }

    private static void CheckForRoutingPort(ContainerInfo curatedInfo)
    {
        if (curatedInfo.PortsInfo.RoutingPortNumber.Equals(default))
            throw new Exception(
            $"""
                            Port not found on container named: {curatedInfo.Name}
                            Make sure the container has ports defined in the compose and if the internal port is not 80, add it to the parameter 'CUSTOM_CONTAINER_PORTS_DISCOVERY' on the NodeController.
                            e.g. CUSTOM_CONTAINER_PORTS_DISCOVERY: 5672, 5432⁠
                            """);
    }

    private static string GetArtifactName(ContainerInfo infos, IContainerConfig rawConfig)
    {
        var value = GetLabelValue(ServiceLabelsEnum.ARTIFACT_NAME, infos.Labels);

        return string.IsNullOrEmpty(value) ? GetArtifactNameFromEnvironmentId(infos, rawConfig) : value;
    }

    private static string GetArtifactNameFromEnvironmentId(ContainerInfo infos, IContainerConfig rawConfig)
    {
        var podNameAndId = rawConfig.Config.Config.Env
            .FirstOrDefault(e => e.ToString().StartsWith("ID="))?
            .Split("=").LastOrDefault() ?? throw new Exception($"ID environment variable not defined in compose for container named: {infos.Name}");

        var id = podNameAndId.Split(".") is { Length: > 1 } podNameAndIdArray
            ? podNameAndIdArray.LastOrDefault() ?? throw new Exception($"ID environment variable not defined in compose for container named: {infos.Name}")
            : podNameAndId;

        return id;
    }

    private static string GetArtifactCategory(ContainerInfo infos)
    {
        var value = GetLabelValue(ServiceLabelsEnum.ARTIFACT_CATEGORY, infos.Labels);

        return string.IsNullOrEmpty(value) ? throw new Exception($"Artifact category not defined in compose for container named: {infos.Name}") : value;
    }

    private static int GetNumberOfInstancesAsNumber(ContainerInfo infos)
    {
        uint.TryParse(GetLabelValue(ServiceLabelsEnum.REPLICAS, infos.Labels), out var nbInstances);

        return Convert.ToInt32(nbInstances);
    }

    private static IEnumerable<string> GetNumberOfInstancesAsHostNames(ContainerInfo infos)
    {
        return GetLabelValue(ServiceLabelsEnum.REPLICAS, infos.Labels)?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)?
            .ToArray()
               ?? Enumerable.Empty<string>();
    }

    private static string GetPodName(ContainerInfo infos, IContainerConfig rawConfig)
    {
        var value = GetLabelValue(ServiceLabelsEnum.POD_NAME, infos.Labels);

        return string.IsNullOrWhiteSpace(value) ? GetPodNameFromEnvironmentId(infos, rawConfig) : value;
    }

    private static string GetPodNameFromEnvironmentId(ContainerInfo infos, IContainerConfig rawConfig)
    {
        return rawConfig.Config.Config.Env
            .FirstOrDefault(e => e.ToString().StartsWith("ID="))?
            .Split("=").LastOrDefault()?
            .Split(".").FirstOrDefault() ?? throw new Exception($"Pod name not defined for container named: {infos.Name}");
    }

    private static string? GetPodId(ContainerInfo infos)
    {
        return GetLabelValue(ServiceLabelsEnum.POD_ID, infos.Labels);
    }

    private static string GetDnsAccessibilityModifier(ContainerInfo infos)
    {
        var value = GetLabelValue(ServiceLabelsEnum.DNS, infos.Labels);

        return string.IsNullOrEmpty(value) ? nameof(AccessibilityModifierEnum.Private) : value;
    }

    private static IEnumerable<string> GetPodLinks(ContainerInfo infos)
    {
        return GetLabelValue(ServiceLabelsEnum.POD_LINKS, infos.Labels)?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)?
            .ToArray()
               ?? Enumerable.Empty<string>();
    }

    private static bool GetShareVolumesWithReplicas(ContainerInfo infos)
    {
        bool.TryParse(GetLabelValue(ServiceLabelsEnum.SHARE_VOLUMES, infos.Labels), out var shareVolumes);

        return shareVolumes;
    }

    private static string? GetLabelValue(ServiceLabelsEnum serviceLabels, ConcurrentDictionary<ServiceLabelsEnum, string> labels)
    {
        labels.TryGetValue(serviceLabels, out var label);

        return label;
    }
}