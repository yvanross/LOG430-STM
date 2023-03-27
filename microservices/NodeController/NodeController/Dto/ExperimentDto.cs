using ApplicationLogic.Interfaces.Dao;
using Cassandra.Mapping;
using Cassandra.Mapping.Attributes;
using Entities.DomainInterfaces.Planned;
using Entities.DomainInterfaces.ResourceManagement;
using NodeController.External.Dao;

namespace NodeController.Dto;

public class ExperimentDto
{
    [ClusteringKey(0, SortOrder.Ascending)]
    public DateTime Timestamp { get; init; }

    public List<ArtifactDto> RunningInstances { get; init; }

    public double AverageLatency { get; init; }

    public int ErrorCount { get; init; }

    public string Message { get; init; }

    public static ExperimentDto TryConvertToDto(IExperimentReport experimentReport)
    {
        return new ExperimentDto
        {
            Timestamp = experimentReport.Timestamp,
            AverageLatency = experimentReport.ExperimentResult?.AverageLatency ?? -1.0,
            ErrorCount = experimentReport.ExperimentResult?.ErrorCount ?? default,
            Message = experimentReport.ExperimentResult?.Message ?? string.Empty,
            RunningInstances = experimentReport.RunningInstances.ConvertAll(ri => ArtifactDto.TryConvertToDto(ri, GetServiceType(ri.Type)))
                                                                    .Where(artifactDto => artifactDto is not null).ToList()!
        };
    }

    private static readonly IPodReadModel PodReadModel = new PodReadModel();

    private static IServiceType? GetServiceType(string serviceType) => PodReadModel.GetServiceType(serviceType);
}