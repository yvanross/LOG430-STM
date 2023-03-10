using ApplicationLogic.Interfaces;
using Entities.Concretions;
using Entities.Domain;
using Microsoft.Extensions.Logging;
using STM.ExternalServiceProvider.Proto;

namespace ApplicationLogic.Services.BusTracking;

public class BusTrackingService : ABusTrackingService
{
    private DateTime _crossedFirstStopTime;

    public BusTrackingService(IBus bus, IStmClient stmClient, ILogger? logger, DateTime crossedFirstStopTime) : base(bus, stmClient, logger)
    {
        _crossedFirstStopTime = crossedFirstStopTime;
    }

    private protected override (IBusTracking, ITrackingService?) Track()
    {
        if (CurrentStopIndex >= Bus.Trip.RelevantDestination!.Value.Index)
        {
            return (new Entities.Concretions.BusTracking()
            {
                Message = $"Real Time Tracking is done, bus {Bus.Name} has reached the destination in {DeltaTime(_crossedFirstStopTime)}"
            }, default);
        }

        var prediction = Predictions(Convert.ToDouble(CurrentStopIndex), Bus.Trip.RelevantOrigin!.Value.Index, Bus.Trip.RelevantDestination!.Value.Index);

        return (new Entities.Concretions.BusTracking()
        {
            Message =
                $"Bus {Bus.Name} is on its way to the target stop, it did {prediction * 100}% of the way in {DeltaTime(_crossedFirstStopTime)} seconds"
        }, this);
    }
}