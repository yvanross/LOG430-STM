using System.Collections.Immutable;
using Entities.Concretions;
using Entities.Domain;
using Google.Protobuf.WellKnownTypes;
using RestSharp;
using StaticGTFS;
using STM.ApplicationLogic;
using STM.Controllers;
using STM.Entities.Concretions;
using STM.Entities.Domain;
using STM.Entities.DTO;
using STM.ExternalServiceProvider;
using Timer = System.Timers.Timer;

namespace STM.Use_Cases;

public class TrackBus : ChaosDaemon
{
    public Func<DateTime?, double> DeltaTime { get; } = (crossedOriginTime)
        => (DateTime.UtcNow - (crossedOriginTime ?? DateTime.UtcNow)).TotalSeconds;

    public bool HasPassedOriginStop { get; private set; }
   
    public bool HasPassedTargetStop { get; private set; }

    #region PrivateVariables

    private IBus _bus { get; set; }

    private uint? _firstStopIndexRecorded { get; set; }

    private int _originStopIndex { get; init; }

    private int _targetStopIndex { get; init; }

    private string? _callBack { get; init; }

    private string? _key { get; init; }

    private uint? _currentStopIndex;

    //predictions
    private DateTime? _lastCheckPoint;
    //

    private DateTime? _crossedOriginTime;

    private DateTime? _crossedFirstStopTime;

    private PeriodicTimer _periodicTimer = new(TimeSpan.FromMinutes(0.25));

    private ExternalSTMGateway _externalSTMGateway = new();

    private ILogger? _logger;

    private RestClient restClient = new RestClient("http://10.194.33.155.nip.io:49160");

    private double _stmETA;

    #endregion

    public TrackBus(TrackingBusDTO trackingBus, ILogger logger)
    {
        IsChaosEnabled();

        var relevantOrigin = trackingBus.OriginStopSchedule;
        var relevantDestination = trackingBus.TargetStopSchedule;

        _stmETA = Convert.ToDouble(trackingBus.ETA);

        _bus = new Bus()
        {
            ID = trackingBus.BusID,
            Name = trackingBus.Name,
            Trip = new TripSTM()
            {
                RelevantOrigin = new StopScheduleSTM()
                {
                    index = relevantOrigin.index,
                    DepartureTime = Convert.ToDateTime(relevantOrigin.DepartureTime),
                    Stop = new StopSTM()
                    {
                        Message = relevantOrigin.Stop.Message,
                        ID = relevantOrigin.Stop.ID,
                        Position = new Position()
                        {
                            Latitude = Convert.ToDouble(relevantOrigin.Stop.Position.Latitude),
                            Longitude = Convert.ToDouble(relevantOrigin.Stop.Position.Longitude)
                        }
                    }
                },
                RelevantDestination = new StopScheduleSTM()
                {
                    index = relevantDestination.index,
                    DepartureTime = Convert.ToDateTime(relevantDestination.DepartureTime),
                    Stop = new StopSTM()
                    {
                        Message = relevantDestination.Stop.Message,
                        ID = relevantDestination.Stop.ID,
                        Position = new Position()
                        {
                            Latitude = Convert.ToDouble(relevantDestination.Stop.Position.Latitude),
                            Longitude = Convert.ToDouble(relevantDestination.Stop.Position.Longitude)
                        }
                    }
                },
                ID = trackingBus.TripID,
            },
        };

        _originStopIndex = _bus.Trip.RelevantOrigin.Value.index;

        _targetStopIndex = _bus.Trip.RelevantDestination.Value.index;

        _callBack = trackingBus.callBack;

        _logger = logger;

        _key = trackingBus.processID;

        _crossedFirstStopTime = DateTime.UtcNow;

    }

    public async Task PerdiodicCaller()
    {
        _logger?.Log(LogLevel.Information, new EventId(0), message: "Beginning Tracking");

        while (await _periodicTimer.WaitForNextTickAsync())
        {
            IsChaosEnabled();

            Track();
        }
    }

    private async Task Track()
    {
        var feedPositions = (await _externalSTMGateway.RequestFeedPositions()).ToImmutableDictionary(v=>v.Vehicle.Id);

        var liveFeed = feedPositions[_bus.ID];

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

                ProvideUpdate($"Bus {_bus.Name} is at the first stop, begin timer");

                return;
            }

            var prediction = Predictions(_currentStopIndex.Value, _firstStopIndexRecorded.Value, _originStopIndex,
                _crossedFirstStopTime.Value, _lastCheckPoint.Value);


            ProvideUpdate($"Bus {_bus.Name} is not yet at the first stop, it did {prediction .progression* 100}% of the way in {DeltaTime(_crossedFirstStopTime)} seconds, expected to reach the first stop in {_stmETA - DeltaTime(_crossedFirstStopTime)} seconds");
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

            ProvideUpdate($"Bus {_bus.Name} is on its way to the target stop, it did {prediction.progression * 100}% of the way in {DeltaTime(_crossedOriginTime)} seconds, expected to reach the destination stop in {prediction.eta} seconds");
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
        var request = new RestRequest(_callBack);

        var deltaTime = DeltaTime(_crossedOriginTime).ToString();

        request.AddQueryParameter("key", _key);
        request.AddQueryParameter("updateMessage", message);
        request.AddQueryParameter("deltaTime", deltaTime);
        request.AddQueryParameter("done", lastCallback);

        var response = await restClient.ExecutePostAsync(request);

        _logger?.LogInformation($"{message}");
    }

    private async Task CallBackAndDispose()
    {
        if (_callBack is not null)
        {
            await ProvideUpdate($"Real Time Tracking is done, bus {_bus.Name} has reached the destination in {DeltaTime(_crossedOriginTime)}", true);
        }

        _logger?.Log(LogLevel.Information, new EventId(5), message: "Disposing");

        _periodicTimer.Dispose();

        _bus = null;
        _logger = null;
        _externalSTMGateway = null;
    }
}