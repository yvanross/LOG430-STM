using ApplicationLogic.Interfaces.Dao;
using Entities.DomainInterfaces.Planned;
using Entities.DomainInterfaces.ResourceManagement;
using NodeController.External.Dao;

namespace NodeController.Dto.OutGoing;

public class ExperimentReportDto
{
    public DateTime Timestamp { get; set; }

    public List<ArtifactDto> RunningInstances { get; set; }

    public double AverageLatency { get; set; }

    public int ErrorCount { get; set; }

    public string Message { get; set; }

    public static ExperimentReportDto TryConvertToDto(IExperimentReport experimentReport)
    {
        return new ExperimentReportDto
        {
            Timestamp = experimentReport.Timestamp,
            AverageLatency = experimentReport.ExperimentResult?.AverageLatency ?? -1.0,
            ErrorCount = experimentReport.ExperimentResult?.ErrorCount ?? default,
            Message = experimentReport.ExperimentResult?.Message ?? string.Empty,
            RunningInstances = experimentReport.RunningInstances.ConvertAll(ri => ArtifactDto.TryConvertToDto(ri, GetServiceType(ri.Type)))
                                                                    .Where(artifactDto => artifactDto is not null).ToList()!
        };
    }

    private static readonly IPodReadService PodReadService = new PodReadService();

    private static IServiceType? GetServiceType(string serviceType) => PodReadService.GetServiceType(serviceType);
}