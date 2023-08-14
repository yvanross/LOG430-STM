using System.Collections.Immutable;
using ApplicationLogic.Interfaces;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Services.BusTracking;

public abstract class ABusTrackingService
{
    internal IBus? Bus { get; set; }

    private protected readonly IStmClient StmClient;
    private protected readonly ILogger? Logger;

    private protected uint CurrentStopIndex;

    protected ABusTrackingService(IStmClient stmClient, ILogger logger)
    {
        StmClient = stmClient;
        Logger = logger;
    }

    public (IBusTracking, ABusTrackingService?) GetUpdate()
    {
        UpdateStopIndex();

        var tracking = Track();

        return tracking;
    }

    private void UpdateStopIndex()
    {
        var feedPositions = StmClient.RequestFeedPositions().ToImmutableDictionary(v => v.Vehicle.Id);

        var liveFeed = feedPositions[Bus.Id];

        CurrentStopIndex = liveFeed.CurrentStopSequence - Convert.ToUInt32(Bus.StopIndexAtComputationTime);
    }

    private protected double Predictions(double currentStop, double firstStop, double targetStop)
    {
        var progression = (currentStop - firstStop) / (targetStop - firstStop);

        return progression;
    }

    private protected TimeSpan DeltaTime(DateTime? crossedOriginTime)
        => (DateTime.UtcNow - (crossedOriginTime ?? DateTime.UtcNow));

    private protected abstract (IBusTracking, ABusTrackingService?) Track();
}