using ApplicationLogic.Interfaces.Dao;
using Entities.BusinessObjects.Live;
using Entities.DomainInterfaces.Live;

namespace ApplicationLogic.Services;

public class ChaosTestMonitoringService
{
    public IExperimentResult? ExperimentResult { get; private set; }

    private int _processedSagaCount;

    private int _sagaDuration;

    private int _sagaPhaseDuration;

    private int _sagaPhase;
    
    private int _errorCount;

    public async Task AnalyzeAndStoreRealtimeTestData(ISaga saga)
    {
        UpdateSagaDuration(saga);
     
        _processedSagaCount++;

        this.ExperimentResult = new ExperimentResult()
        {
            AverageLatency = GetAverageLatency(),
            ErrorCount = _errorCount,
            Message = saga.Message
        };

        double GetAverageLatency()
        {
            return Convert.ToDouble(_sagaDuration) / _processedSagaCount;
        }

        void UpdateSagaDuration(ISaga data)
        {
            if (data.Phase.Equals(_sagaPhase))
            {
                if (_sagaPhaseDuration > data.Seconds)
                    _errorCount++;

                _sagaPhaseDuration = data.Seconds;
            }
            else
            {
                _sagaPhase = data.Phase;

                _sagaDuration += _sagaPhaseDuration;
            }
        }
    }
}