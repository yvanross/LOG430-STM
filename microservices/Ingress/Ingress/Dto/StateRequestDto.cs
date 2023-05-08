using Microsoft.Extensions.ObjectPool;

namespace ApplicationLogic.Dto;

public class StateRequestDto
{
    public required object ExperimentReportDto { get; set; }

    public required string State { get; set; }
    
    public required string Version { get; set; }

    public required bool Secure { get; set; }

    public required bool Dirty { get; set; }
}