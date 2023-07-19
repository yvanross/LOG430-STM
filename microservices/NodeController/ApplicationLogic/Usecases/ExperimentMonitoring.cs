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
    private static MessageProcessor _messageProcessor = new ();

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
        _messageProcessor.StartProcessing();

        _messageProcessor.OnMessageProcessed();

        if (GetSagaDuration() > busPositionUpdated.Seconds)
            IncrementTestErrorCount();

        SetSagaDuration(busPositionUpdated.Seconds);

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
                AverageLatency = _messageProcessor.GetAverageOfProcessingTimes(),
                ErrorCount = GetTestErrorCount(),
                Message = GetMessage()
            }
        });
    }

    //private double GetAverageLatency()
    //{
    //    return Convert.ToDouble(_stopwatch?.Elapsed.TotalMilliseconds ?? 1) / Convert.ToDouble(GetProcessedMessageCount());
    //}


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

    //private class MessageProcessor
    //{
    //    private const int ResetInterval = 5_000;

    //    private readonly Stopwatch _stopwatch = new();
    //    private readonly ConcurrentBag<double> _processingTimesAverageDeviations = new();
    //    private readonly PeriodicTimer _timer = new(period: TimeSpan.FromMilliseconds(ResetInterval));
    //    private readonly CancellationTokenSource _cancellationTokenSource = new();

    //    private int _messagesProcessedDuringInterval = 0;

    //    public void StartProcessing()
    //    {
    //        if (_stopwatch.IsRunning) return;

    //        _stopwatch.Start();
    //        _ = ResetProcessingTimesPeriodically();
    //    }

    //    public void MessageProcessed()
    //    {
    //        Interlocked.Increment(ref _messagesProcessedDuringInterval);
    //    }

    //    public double GetProcessingTimesCumulativeOfAverageDeviations()
    //    {
    //        return _processingTimesAverageDeviations.Average();
    //    }

    //    private async Task ResetProcessingTimesPeriodically()
    //    {
    //        while (await _timer.WaitForNextTickAsync(_cancellationTokenSource.Token))
    //        {
    //            CalculateProcessingTimeStandardDeviation();
    //            _stopwatch.Reset();
    //            _stopwatch.Start();
    //            Interlocked.Exchange(ref _messagesProcessedDuringInterval, 0);
    //        }
    //    }

    //    private void CalculateProcessingTimeStandardDeviation()
    //    {
    //        var average = CalculateAverageProcessingTime();

    //        _processingTimesAverageDeviations.Add(average);
    //    }

    //    private double CalculateAverageProcessingTime()
    //    {
    //        return Math.Min(_stopwatch.Elapsed.TotalMilliseconds / _messagesProcessedDuringInterval, ResetInterval);
    //    }
    //}

    private class MessageProcessor : IDisposable
    {
        private const int IntervalToResetProcessingTime = 5_000;

        private readonly Stopwatch _processingTimeStopwatch = new();
        private readonly ConcurrentBag<double> _averageProcessingTimes = new();
        private readonly PeriodicTimer _resetTimer = new(period: TimeSpan.FromMilliseconds(IntervalToResetProcessingTime));
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        private int _messageCountInCurrentInterval = 0;

        public void StartProcessing()
        {
            if (_processingTimeStopwatch.IsRunning) return;

            _processingTimeStopwatch.Start();

            _ = ResetProcessingTimesPeriodicallyAsync();
        }

        public void OnMessageProcessed()
        {
            Interlocked.Increment(ref _messageCountInCurrentInterval);
        }

        public double GetAverageOfProcessingTimes()
        {
            return _averageProcessingTimes.Average();
        }

        private async Task ResetProcessingTimesPeriodicallyAsync()
        {
            while (await _resetTimer.WaitForNextTickAsync(_cancellationTokenSource.Token))
            {
                ComputeAndStoreAverageProcessingTime();
                ResetProcessingTimeAndCount();
            }
        }

        private void ComputeAndStoreAverageProcessingTime()
        {
            var averageProcessingTimeInCurrentInterval = CalculateAverageProcessingTime();

            _averageProcessingTimes.Add(averageProcessingTimeInCurrentInterval);
        }

        private void ResetProcessingTimeAndCount()
        {
            _processingTimeStopwatch.Restart();

            Interlocked.Exchange(ref _messageCountInCurrentInterval, 0);
        }

        private double CalculateAverageProcessingTime()
        {
            return Math.Min(_processingTimeStopwatch.Elapsed.TotalMilliseconds / _messageCountInCurrentInterval, IntervalToResetProcessingTime);
        }

        public void Dispose()
        {
            _resetTimer.Dispose();
            _cancellationTokenSource.Dispose();
        }
    }

}