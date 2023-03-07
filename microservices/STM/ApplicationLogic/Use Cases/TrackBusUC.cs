using System.Collections.Immutable;
using ApplicationLogic.Interfaces;
using Entities.Domain;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Use_Cases;

public class TrackBusUC
{
    public bool HasPassedOriginStop { get; private set; }
   
    public bool HasPassedTargetStop { get; private set; }

    #region PrivateVariables

    private readonly IBus _bus;

    private uint? _firstStopIndexRecorded { get; set; }

    private int _originStopIndex { get; init; }

    private int _targetStopIndex { get; init; }

    private uint? _currentStopIndex;

    //predictions
    private DateTime? _lastCheckPoint;
    //

    private DateTime? _crossedOriginTime;

    private readonly DateTime? _crossedFirstStopTime;

    private readonly PeriodicTimer _periodicTimer = new(TimeSpan.FromSeconds(1));

    private IStmClient _stmClient;

    private readonly ILogger? _logger;

    private readonly ICallBackClient? _callBackClient;

    private readonly double _eta;

    #endregion

    public TrackBusUC(IBus bus, double eta, ICallBackClient? callBackClient, IStmClient stmClient, ILogger? logger)
    {
        _bus = bus;
        _eta = eta;
        _callBackClient = callBackClient;
        _stmClient = stmClient;
        _logger = logger;

        _crossedFirstStopTime = DateTime.UtcNow;

        _originStopIndex = bus.Trip.RelevantOrigin!.Value.Index;
        _targetStopIndex = bus.Trip.RelevantDestination!.Value.Index;

        _ = PerdiodicCaller();
    }

    public async Task PerdiodicCaller()
    {
        _logger?.Log(LogLevel.Information, new EventId(0), message: "Beginning Tracking");

        while (await _periodicTimer.WaitForNextTickAsync())
        {
            await Track();
        }
    }

    private async Task Track()
    {
        var feedPositions = _stmClient.RequestFeedPositions().ToImmutableDictionary(v=>v.Vehicle.Id);

        var liveFeed = feedPositions[_bus.Id];

        if (_bus.Trip.FromStaticGtfs is false)
            liveFeed.CurrentStopSequence -= (uint)_bus.currentStopIndex;

        _firstStopIndexRecorded ??= liveFeed.CurrentStopSequence;

        if (_currentStopIndex is null || liveFeed.CurrentStopSequence > _currentStopIndex)
        {
            _currentStopIndex = liveFeed.CurrentStopSequence;
            _lastCheckPoint = DateTime.UtcNow;
        }

        if (!HasPassedOriginStop)
        {
            if (liveFeed.CurrentStopSequence >= _originStopIndex)
            {
                HasPassedOriginStop = true;

                _crossedOriginTime = DateTime.UtcNow;

                await ProvideUpdate($"Bus {_bus.Name} is at the first stop, begin timer");

                return;
            }

            var prediction = Predictions(_currentStopIndex.Value, _firstStopIndexRecorded.Value, _originStopIndex,
                _crossedFirstStopTime.Value, _lastCheckPoint.Value);

            await ProvideUpdate($"Bus {_bus.Name} is not yet at the first stop, it did {prediction .progression* 100}% of the way in {DeltaTime(_crossedFirstStopTime)} seconds, expected to reach the first stop in {_eta - DeltaTime(_crossedFirstStopTime)} seconds");
        }
        else if (!HasPassedTargetStop)
        {
            if (liveFeed.CurrentStopSequence >= _targetStopIndex)
            {
                HasPassedTargetStop = true;

                await CallBackAndDispose();

                return;
            }

            var prediction = Predictions(_currentStopIndex.Value, _originStopIndex, _targetStopIndex,
                _crossedOriginTime.Value, _lastCheckPoint.Value);

            await ProvideUpdate($"Bus {_bus.Name} is on its way to the target stop, it did {prediction.progression * 100}% of the way in {DeltaTime(_crossedOriginTime)} seconds, expected to reach the destination stop in {prediction.eta} seconds");
        }
    }

    private (double progression, double eta) Predictions(double referenceStopB, double referenceStopA, double referenceStopC, DateTime timeA, DateTime timeB)
    {
        var progression = (referenceStopB - referenceStopA) / (referenceStopC - referenceStopA);

        var staticETA = ((timeB - timeA).TotalSeconds / progression) - (timeB - timeA).TotalSeconds;

        var eta = staticETA - (DateTime.UtcNow - timeB).TotalSeconds;

        return new (progression, eta);
    }

    private async Task ProvideUpdate(string message, bool lastCallback = false)
    {
        _logger?.Log(LogLevel.Information, new EventId(0), message: message);

        var deltaTime = DeltaTime(_crossedOriginTime).ToString();

        if(_callBackClient is not null)
            await _callBackClient.CallBack(message, deltaTime, lastCallback);
    }

    private async Task CallBackAndDispose()
    {
        await ProvideUpdate($"Real Time Tracking is done, bus {_bus.Name} has reached the destination in {DeltaTime(_crossedOriginTime)}", true);

        _logger?.Log(LogLevel.Information, new EventId(5), message: "Disposing");

        _periodicTimer.Dispose();
    }

    private double DeltaTime(DateTime? crossedOriginTime)
        => (DateTime.UtcNow - (crossedOriginTime ?? DateTime.UtcNow)).TotalSeconds;
}