using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Dao;
using Entities.BusinessObjects.Live;
using Entities.BusinessObjects.ResourceManagement;
using Entities.DomainInterfaces.Live;

namespace ApplicationLogic.Usecases;

public class ExperimentMonitoringUC
{
    public IExperimentResult? ExperimentResult { get; private set; }

    private readonly ISystemStateWriteService _systemStateWriteService;

    private readonly IPodReadService _readServiceService;

    private int _processedSagaCount;

    private int _sagaDuration;

    private int _errorCount;

    public ExperimentMonitoringUC(ISystemStateWriteService systemStateWriteService, IPodReadService readServiceService)
    {
        _systemStateWriteService = systemStateWriteService;
        _readServiceService = readServiceService;
    }

    public void AnalyzeAndStoreRealtimeTestData(IBusPositionUpdated busPositionUpdated)
    {
        UpdateSagaDuration(busPositionUpdated);

        ExperimentResult = new ExperimentResult()
        {
            AverageLatency = GetAverageLatency(),
            ErrorCount = _errorCount,
            Message = busPositionUpdated.Message
        };

        double GetAverageLatency()
        {
            return Convert.ToDouble(_sagaDuration / _processedSagaCount);
        }

        void UpdateSagaDuration(IBusPositionUpdated data)
        {
            if (_sagaDuration > data.Seconds)
                _errorCount++;

            _sagaDuration = data.Seconds;

            _processedSagaCount++;
        }
    }

    public async Task LogExperimentResults()
    {
        await _systemStateWriteService.Log(new ExperimentReport()
        {
            ServiceTypes = _readServiceService.GetAllServiceTypes().ToList(),
            RunningInstances = _readServiceService.GetAllServices().ToList(),
            ExperimentResult = ExperimentResult
        });
    }
}