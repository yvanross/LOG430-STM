using System.Collections.Immutable;
using ApplicationLogic.Interfaces;
using Entities.Domain;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Services.BusTracking;

public abstract class ABusTrackingService : ITrackingService
{

    private protected readonly IBus Bus;

    private protected IStmClient StmClient;

    private protected readonly ILogger? Logger;

    private protected uint CurrentStopIndex;

    protected ABusTrackingService(IBus bus, IStmClient stmClient, ILogger? logger)
    {
        Bus = bus;
        StmClient = stmClient;
        Logger = logger;
    }

    public (IBusTracking, ITrackingService?) GetUpdate()
    {
        UpdateStopIndex();

        var tracking = Track();

        Logger.LogInformation(tracking.Item1.Message);

        return tracking;
    }

    private protected abstract (IBusTracking, ITrackingService?) Track();

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
}