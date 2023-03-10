using ApplicationLogic.Interfaces;
using Entities.Concretions;
using Entities.Domain;
using Microsoft.Extensions.Logging;
using STM.ExternalServiceProvider.Proto;

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
            var busTrackingService = new BusTrackingService(Bus, StmClient, Logger, DateTime.UtcNow);

            return (new Entities.Concretions.BusTracking()
            {
                Message = $"Bus {Bus.Name} is at the first stop, begin timer"
            }, busTrackingService);
        }

        var prediction = Predictions(Convert.ToDouble(CurrentStopIndex), Convert.ToDouble(_firstStopIndexRecorded), Bus.Trip.RelevantOrigin!.Value.Index);

        return (new Entities.Concretions.BusTracking()
        {
            Message = $"Bus {Bus.Name} is not yet at the first stop, it did {prediction * 100}% of the way in {DeltaTime(_startingTime)} seconds, expected to reach the first stop in {_originalEta - DeltaTime(_startingTime)} seconds"
        }, this);
    }
}