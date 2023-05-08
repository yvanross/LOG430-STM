using ApplicationLogic.Interfaces;
using Entities.Domain;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Services.BusTracking;

public class BeforeFirstStopTrackingService : ABusTrackingService
{
    private readonly DateTime _startingTime = DateTime.UtcNow;

    private readonly double _originalEta;

    private uint? _firstStopIndexRecorded;

    public BeforeFirstStopTrackingService(IBus bus, IStmClient stmClient, ILogger? logger, double originalEta) : base(bus, stmClient, logger)
    {
        _originalEta = originalEta;
    }

    private protected override (IBusTracking, ITrackingService?) Track()
    {
        _firstStopIndexRecorded ??= CurrentStopIndex;

        if (CurrentStopIndex >= Bus.Trip.RelevantOrigin!.Value.Index)
        {
            var busTrackingService = new BusTrackingService(Bus, StmClient, Logger, DateTime.UtcNow, _startingTime);

            return (new Entities.Concretions.BusTracking()
            {
                Message = $"Bus {Bus.Name} is at the first stop, begin timer",
                Duration = DeltaTime(_startingTime).TotalMilliseconds,
            }, busTrackingService);
        }

        var prediction = Predictions(Convert.ToDouble(CurrentStopIndex), Convert.ToDouble(_firstStopIndexRecorded), Bus.Trip.RelevantOrigin!.Value.Index);

        return (new Entities.Concretions.BusTracking()
        {
            Message = $"Bus {Bus.Name}:\nNot yet at first stop.\n{Convert.ToInt32(prediction * 100)}% in {Convert.ToInt32(DeltaTime(_startingTime).TotalSeconds)} seconds",
            Duration = DeltaTime(_startingTime).TotalMilliseconds,
        }, this);
    }
}