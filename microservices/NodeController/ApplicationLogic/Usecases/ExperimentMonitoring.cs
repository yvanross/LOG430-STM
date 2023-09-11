using System.Diagnostics;
using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Dao;
using Entities.BusinessObjects.Live;
using Entities.BusinessObjects.ResourceManagement;
using Entities.Dao;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Usecases;

public class ExperimentMonitoring
{
    // This is static because there is only one experiment running per service lifetime.
    private static MessageProcessor? MessageProcessingService;

    private readonly ISystemStateWriteService _systemStateWriteService;

    private readonly IPodReadService _readService;

    private protected readonly ILogger<ExperimentMonitoring> _logger;

    private static int _testErrorCount;

    private static int _sagaDuration;

    private static int _processedMessageCount;
    
    private static string _message = "";

    public ExperimentMonitoring(ISystemStateWriteService systemStateWriteService, IPodReadService readService, ILogger<ExperimentMonitoring> logger)
    {
        _systemStateWriteService = systemStateWriteService;
        _readService = readService;
        MessageProcessingService ??= new MessageProcessor(logger);
        _logger = logger;
    }

    public void AnalyzeAndStoreRealtimeTestData(IBusPositionUpdated busPositionUpdated)
    {
        MessageProcessingService.StartProcessing();

        MessageProcessingService.MessageProcessed();

        if (GetExepirmentDuration() > busPositionUpdated.Seconds)
            IncrementTestErrorCount();

        SetExperimentDuration(busPositionUpdated.Seconds);

        IncrementProcessedMessageCount();

        SetMessage(busPositionUpdated.Message);
    }

    public async Task LogExperimentResults()
    {
        await _systemStateWriteService.Log(new ExperimentReport()
        {
            ServiceTypes = _readService.GetAllServiceTypes().ToList(),
            RunningInstances = _readService.GetAllServices().ToList(),
            ExperimentResult = new ExperimentResult()
            {
                AverageLatency = MessageProcessingService.GetAverageTimeBetweenMessages(),
                Stability = MessageProcessingService.GetStandardDeviation(),
                ErrorCount = GetTestErrorCount(),
                Message = GetMessage()
            }
        });
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

    private void SetExperimentDuration(int seconds)
    {
        Interlocked.Exchange(ref _sagaDuration, seconds);
    }

    private void ResetExperimentDuration()
    {
        Interlocked.Exchange(ref _sagaDuration, 0);
    }

    private int GetExepirmentDuration() => _sagaDuration;

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

    //private class MessageProcessor
    //{
    //    private const int ResetInterval = 5_000;

    //    private readonly Stopwatch _stopwatch = new();
    //    private readonly ConcurrentBag<double> _timeDeltas = new();
    //    private readonly ConcurrentBag<double> _processingTimeStdDevs = new();
    //    private readonly PeriodicTimer _timer = new(period: TimeSpan.FromMilliseconds(ResetInterval));
    //    private readonly CancellationTokenSource _cancellationTokenSource = new();

    //    public void StartProcessing()
    //    {
    //        _stopwatch.Start();
    //        _ = ResetProcessingTimesPeriodically();
    //    }

    //    public void MessageProcessed()
    //    {
    //        long currentTime = _stopwatch.ElapsedMilliseconds;
    //        _timeDeltas.Add(currentTime);
    //        _stopwatch.Restart();
    //    }

    //    public double GetAverageTimeBetweenMessages()
    //    {
    //        return _timeDeltas.Count > 0 ? _timeDeltas.Average() : double.MaxValue;
    //    }

    //    public double GetAverageStandardDeviation()
    //    {
    //        return _processingTimeStdDevs.Count > 0 ? _processingTimeStdDevs.Average() : 0;
    //    }

    //    private async Task ResetProcessingTimesPeriodically()
    //    {
    //        while (await _timer.WaitForNextTickAsync(_cancellationTokenSource.Token))
    //        {
    //            double stdDev = CalculateStandardDeviation();
    //            _processingTimeStdDevs.Add(stdDev);
    //        }
    //    }

    //    private double CalculateStandardDeviation()
    //    {
    //        if (_timeDeltas.Count < 2)
    //        {
    //            return ResetInterval;
    //        }

    //        double average = _timeDeltas.Average();
    //        double sumOfSquaresOfDifferences = _timeDeltas.Select(val => (val - average) * (val - average)).Sum();
    //        double stdDev = Math.Sqrt(sumOfSquaresOfDifferences / (_timeDeltas.Count - 1));

    //        return stdDev;
    //    }
    //}

    private class MessageProcessor
    {
        private readonly ILogger _logger;
        private readonly Stopwatch _totalElapsedStopwatch = new();
        private readonly Stopwatch _intervalElapsedStopwatch = new();

        private long _sumOfSquareDifferences = 0;
        private long _count = 0;

        public MessageProcessor(ILogger logger)
        {
            _logger = logger;
        }

        public void StartProcessing()
        {
            _intervalElapsedStopwatch.Start();
            _totalElapsedStopwatch.Start();
        }

        public void MessageProcessed()
        {
            long currentInterval = _intervalElapsedStopwatch.ElapsedMilliseconds;

            _intervalElapsedStopwatch.Restart();

            long previousSumOfSquareDifferences = Interlocked.Read(ref _sumOfSquareDifferences);
            long localCount = Interlocked.Increment(ref _count);

            long meanIntervalTime = _totalElapsedStopwatch.ElapsedMilliseconds / localCount;  // Calculate mean locally
            long delta = currentInterval - meanIntervalTime;

            long newSumOfSquareDifferences = previousSumOfSquareDifferences + delta * delta;

            Interlocked.Exchange(ref _sumOfSquareDifferences, newSumOfSquareDifferences);
        }

        public double GetStandardDeviation()
        {
            long localSumOfSquareDifferences = Interlocked.Read(ref _sumOfSquareDifferences);
            long localCount = Interlocked.Read(ref _count);

            var standardDeviation = (localCount > 1) ? Math.Sqrt((double)localSumOfSquareDifferences / (localCount - 1)) : 0;

            _logger.LogInformation($"Standard Deviation: {standardDeviation}");

            return standardDeviation;
        }

        public double GetAverageTimeBetweenMessages()
        {
            long localCount = Interlocked.Read(ref _count);
            long totalElapsedTime = _totalElapsedStopwatch.ElapsedMilliseconds;

            return localCount > 0 ? (double)totalElapsedTime / localCount : 0;
        }
    }

}