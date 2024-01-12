using Application.CommandServices.Interfaces;
using Infrastructure.FileHandlers.StaticGtfs.Enum;

namespace Infrastructure.FileHandlers.StaticGtfs;

public sealed class MemoryConsumptionSettings(MemoryConsumptionEnum memoryConsumptionEnum) : IMemoryConsumptionSettings
{
    private const int LowBatchSize = 1_000;

    private const int MediumBatchSize = 5_000;

    private const int HighBatchSize = 15_000;

    private const int LowReadBatchSize = 10_000;

    private const int MediumReadBatchSize = 100_000;

    private const int HighReadBatchSize = 10_000_000;

    private const int LowMaxDegreeOfParallelism = 1;

    private const int MediumMaxDegreeOfParallelism = 2;

    private const int HighMaxDegreeOfParallelism = 8;

    public int GetBatchSize()
    {
        return memoryConsumptionEnum switch
        {
            MemoryConsumptionEnum.LOW => LowBatchSize,
            MemoryConsumptionEnum.MEDIUM => MediumBatchSize,
            MemoryConsumptionEnum.HIGH => HighBatchSize,
            _ => throw new ArgumentOutOfRangeException(nameof(memoryConsumptionEnum), memoryConsumptionEnum, null)
        };
    }

    public int GetMaxDegreeOfParallelism()
    {
        return memoryConsumptionEnum switch
        {
            MemoryConsumptionEnum.LOW => LowMaxDegreeOfParallelism,
            MemoryConsumptionEnum.MEDIUM => MediumMaxDegreeOfParallelism,
            MemoryConsumptionEnum.HIGH => HighMaxDegreeOfParallelism,
            _ => throw new ArgumentOutOfRangeException(nameof(memoryConsumptionEnum), memoryConsumptionEnum, null)
        };
    }

    public int FileReadBatchSize()
    {
        return memoryConsumptionEnum switch
        {
            MemoryConsumptionEnum.LOW => LowReadBatchSize,
            MemoryConsumptionEnum.MEDIUM => MediumReadBatchSize,
            MemoryConsumptionEnum.HIGH => HighReadBatchSize,
            _ => throw new ArgumentOutOfRangeException(nameof(memoryConsumptionEnum), memoryConsumptionEnum, null)
        };
    }
}