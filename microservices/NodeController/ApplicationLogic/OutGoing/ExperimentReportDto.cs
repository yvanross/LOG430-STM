using ApplicationLogic.Interfaces.Dao;
using Entities.DomainInterfaces.Planned;
using Entities.DomainInterfaces.ResourceManagement;

namespace ApplicationLogic.OutGoing;

public class ExperimentReportDto
{
    public DateTime Timestamp { get; set; }

    public List<ArtifactDto> RunningInstances { get; set; }

    public double AverageLatency { get; set; }

    public int ErrorCount { get; set; }

    public string Message { get; set; }

    public static ExperimentReportDto TryConvertToDto(IExperimentReport experimentReport, IPodReadService podReadService)
    {
        return new ExperimentReportDto
        {
            Timestamp = experimentReport.Timestamp,
            AverageLatency = Math.Round(experimentReport.ExperimentResult?.AverageLatency ?? -1.0, 3),
            ErrorCount = experimentReport.ExperimentResult?.ErrorCount ?? default,
            Message = experimentReport.ExperimentResult?.Message ?? string.Empty,
            RunningInstances = experimentReport.RunningInstances
                .ConvertAll(serviceInstance => ArtifactDto.TryConvertToDto(serviceInstance, GetServiceType(serviceInstance.Type, podReadService)))
                                                                    .Where(artifactDto => artifactDto is not null).ToList()!
        };
    }

    private static IServiceType? GetServiceType(string serviceType, IPodReadService podReadService) => podReadService.GetServiceType(serviceType);
}