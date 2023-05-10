using ApplicationLogic.Interfaces;
using Entities.Transit.Interfaces;
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
        if (CurrentStopIndex >= Bus.TransitTrip.RelevantDestination!.Value.Index)
        {
            return (new Entities.Transit.Concretions.BusTracking()
            {
                Message = $"Real Time Tracking is done, bus {Bus.Name} has reached the destination in {DeltaTime(_crossedFirstStopTime).TotalSeconds}",
                Duration = DeltaTime(_startingTime).TotalMilliseconds,
                TrackingCompleted = true
            }, default);
        }

        var prediction = Predictions(Convert.ToDouble(CurrentStopIndex), Bus.TransitTrip.RelevantOrigin!.Value.Index, Bus.TransitTrip.RelevantDestination!.Value.Index);

        return (new Entities.Transit.Concretions.BusTracking()
        {
            Message = $"Bus {Bus.Name}:\n{Convert.ToInt32(prediction * 100)}% in {Convert.ToInt32(DeltaTime(_crossedFirstStopTime).TotalSeconds)} seconds",
            Duration = DeltaTime(_startingTime).TotalMilliseconds
        }, this);
    }
}