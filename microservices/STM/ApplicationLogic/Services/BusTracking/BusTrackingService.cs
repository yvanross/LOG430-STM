using ApplicationLogic.Interfaces;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Services.BusTracking;

public class BusTrackingService : ABusTrackingService
{
    private readonly DateTime _crossedFirstStopTime;
    private readonly DateTime _startingTime;

    public BusTrackingService(IBus bus, IStmClient stmClient, ILogger logger, DateTime crossedFirstStopTime, DateTime startingTime) : base(stmClient, logger)
    {
        Bus = bus;
        _crossedFirstStopTime = crossedFirstStopTime;
        _startingTime = startingTime;
    }

    private protected override (IBusTracking, ABusTrackingService?) Track()
    {
        if (CurrentStopIndex >= Bus.TransitTripId.RelevantDestination!.Value.Index)
        {
            return (new BusinessObjects.BusTracking()
            {
                Message = $"Real Time Track is done, bus {Bus.Name} has reached the destination in {DeltaTime(_crossedFirstStopTime).TotalSeconds}",
                Duration = DeltaTime(_startingTime).TotalMilliseconds,
                TrackingCompleted = true
            }, default);
        }

        var prediction = Predictions(Convert.ToDouble(CurrentStopIndex), Bus.TransitTripId.RelevantOrigin!.Value.Index, Bus.TransitTripId.RelevantDestination!.Value.Index);

        return (new BusinessObjects.BusTracking()
        {
            Message = $"Bus {Bus.Name}:\n{Convert.ToInt32(prediction * 100)}% in {Convert.ToInt32(DeltaTime(_crossedFirstStopTime).TotalSeconds)} seconds",
            Duration = DeltaTime(_startingTime).TotalMilliseconds
        }, this);
    }
}