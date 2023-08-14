using ApplicationLogic.Interfaces;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Services.BusTracking;

public class BeforeFirstStopTrackingService : ABusTrackingService
{
    private readonly DateTime _startingTime = DateTime.UtcNow;

    private uint? _firstStopIndexRecorded;

    public BeforeFirstStopTrackingService(IStmClient stmClient, ILogger<BeforeFirstStopTrackingService>? logger) : base(stmClient, logger) { }

    private protected override (IBusTracking, ABusTrackingService?) Track()
    {
        _firstStopIndexRecorded ??= CurrentStopIndex;

        if (CurrentStopIndex >= Bus.TransitTripId.RelevantOrigin!.Value.Index)
        {
            var busTrackingService = new BusTrackingService(Bus, StmClient, Logger, DateTime.UtcNow, _startingTime);

            return (new BusinessObjects.BusTracking()
            {
                Message = $"Bus {Bus.Name} is at the first stop, begin timer",
                Duration = DeltaTime(_startingTime).TotalMilliseconds,
            }, busTrackingService);
        }

        var prediction = Predictions(Convert.ToDouble(CurrentStopIndex), Convert.ToDouble(_firstStopIndexRecorded), Bus.TransitTripId.RelevantOrigin!.Value.Index);

        return (new BusinessObjects.BusTracking()
        {
            Message = $"Bus {Bus.Name}:\nNot yet at first stop.\n{Convert.ToInt32(prediction * 100)}% in {Convert.ToInt32(DeltaTime(_startingTime).TotalSeconds)} seconds",
            Duration = DeltaTime(_startingTime).TotalMilliseconds,
        }, this);
    }
}