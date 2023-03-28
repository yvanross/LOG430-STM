using Entities.BusinessObjects.Live;
using Entities.DomainInterfaces.Live;

namespace ApplicationLogic.Services;

public class ChaosTestMonitoringService
{
    public IExperimentResult? ExperimentResult { get; private set; }

    private int _processedSagaCount;

    private int _sagaDuration;

    private int _errorCount;

    public void AnalyzeAndStoreRealtimeTestData(ISaga saga)
    {
        UpdateSagaDuration(saga);
        
        this.ExperimentResult = new ExperimentResult()
        {
            AverageLatency = GetAverageLatency(),
            ErrorCount = _errorCount,
            Message = saga.Message
        };

        double GetAverageLatency()
        {
            return Convert.ToDouble(_sagaDuration / _processedSagaCount);
        }

        void UpdateSagaDuration(ISaga data)
        {
            if (_sagaDuration > data.Seconds)
                _errorCount++;

            _sagaDuration = data.Seconds;

            _processedSagaCount++;
        }
    }
}