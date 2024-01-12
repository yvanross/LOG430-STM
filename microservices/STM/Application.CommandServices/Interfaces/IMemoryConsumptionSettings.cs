namespace Application.CommandServices.Interfaces;

public interface IMemoryConsumptionSettings
{
    int GetBatchSize();
    int GetMaxDegreeOfParallelism();
    int FileReadBatchSize();
}