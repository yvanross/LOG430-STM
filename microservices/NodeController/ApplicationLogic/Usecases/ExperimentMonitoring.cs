using System.Diagnostics;
using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Dao;
using Entities.BusinessObjects.Live;
using Entities.BusinessObjects.ResourceManagement;
using Entities.Dao;

namespace ApplicationLogic.Usecases;

public class ExperimentMonitoring
{
    private readonly ISystemStateWriteService _systemStateWriteService;

    private readonly IPodReadService _readService;

    private static int _testErrorCount;

    private static int _sagaDuration;

    private static int _processedMessageCount;
    
    private static string _message = "";

    private static Stopwatch? _stopwatch;

    private readonly object _lock = new ();

    public ExperimentMonitoring(ISystemStateWriteService systemStateWriteService, IPodReadService readService)
    {
        _systemStateWriteService = systemStateWriteService;
        _readService = readService;
    }

    public void AnalyzeAndStoreRealtimeTestData(IBusPositionUpdated busPositionUpdated)
    {
        if (GetSagaDuration() > busPositionUpdated.Seconds)
            IncrementTestErrorCount();

        SetSagaDuration(busPositionUpdated.Seconds);

        IncrementProcessedMessageCount();

        SetMessage(busPositionUpdated.Message);

        if(_stopwatch is null)
            lock (_lock)
            {
                _stopwatch = Stopwatch.StartNew();
            }
    }

    public async Task LogExperimentResults()
    {
        await _systemStateWriteService.Log(new ExperimentReport()
        {
            ServiceTypes = _readService.GetAllServiceTypes().ToList(),
            RunningInstances = _readService.GetAllServices().ToList(),
            ExperimentResult = new ExperimentResult()
            {
                AverageLatency = GetAverageLatency(),
                ErrorCount = GetTestErrorCount(),
                Message = GetMessage()
            }
        });
    }

    private double GetAverageLatency()
    {
        return Convert.ToDouble(_stopwatch?.Elapsed.TotalMilliseconds ?? 1) / Convert.ToDouble(GetProcessedMessageCount());
    }


    private void IncrementTestErrorCount()
    {
        Interlocked.Increment(ref _testErrorCount);
    }

    private void ResetTestErrorCount()
    {
        Interlocked.Exchange(ref _testErrorCount, 0);
    }

    private int GetTestErrorCount() => _testErrorCount;

    private void SetSagaDuration(int seconds)
    {
        Interlocked.Exchange(ref _sagaDuration, seconds);
    }

    private void ResetSagaDuration()
    {
        Interlocked.Exchange(ref _sagaDuration, 0);
    }

    private int GetSagaDuration() => _sagaDuration;

    private void IncrementProcessedMessageCount()
    {
        Interlocked.Increment(ref _processedMessageCount);
    }

    private void ResetProcessedMessageCount()
    {
        Interlocked.Exchange(ref _processedMessageCount, 0);
    }

    private int GetProcessedMessageCount() => _processedMessageCount;

    private void SetMessage(string message)
    {
        Interlocked.Exchange(ref _message, message);
    }

    private void ResetMessage()
    {
        Interlocked.Exchange(ref _message, string.Empty);
    }

    private string GetMessage() => _message;
}