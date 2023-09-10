using System.Collections.Concurrent;
using System.Diagnostics;
using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Dao;
using Entities.BusinessObjects.Live;
using Entities.BusinessObjects.ResourceManagement;
using Entities.Dao;

namespace ApplicationLogic.Usecases;

public class ExperimentMonitoring
{
    // This is static because there is only one experiment running per service lifetime.
    private static readonly MessageProcessor MessageProcessingService = new ();

    private readonly ISystemStateWriteService _systemStateWriteService;

    private readonly IPodReadService _readService;

    private static int _testErrorCount;

    private static int _sagaDuration;

    private static int _processedMessageCount;
    
    private static string _message = "";

    public ExperimentMonitoring(ISystemStateWriteService systemStateWriteService, IPodReadService readService)
    {
        _systemStateWriteService = systemStateWriteService;
        _readService = readService;
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
        private readonly Stopwatch _stopwatch = new();
        private readonly ConcurrentBag<long> _timeDeltas = new();

        private double _mean = 0;
        private double _m2 = 0;
        private int _count = 0;

        public void StartProcessing()
        {
            _stopwatch.Start();
        }

        public void MessageProcessed()
        {
            long currentTime = _stopwatch.ElapsedMilliseconds;
            _timeDeltas.Add(currentTime);
            _stopwatch.Restart();

            _count++;
            double delta = currentTime - _mean;
            _mean += delta / _count;
            double delta2 = currentTime - _mean;
            _m2 += delta * delta2;
        }

        public double GetStandardDeviation()
        {
            if (_count < 2)
            {
                return 0;
            }

            return Math.Sqrt(_m2 / (_count - 1));
        }

        public double GetAverageTimeBetweenMessages()
        {
            return _timeDeltas.Count > 0 ? _timeDeltas.Average() : 0;
        }
    }
}