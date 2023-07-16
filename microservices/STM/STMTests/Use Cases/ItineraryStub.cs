using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationLogic.Use_Cases;
using Entities.Common.Concretions;
using Entities.Common.Interfaces;
using Entities.Transit.Concretions;
using Entities.Transit.Interfaces;

namespace STMTests.Use_Cases;

public class ItineraryStub : IItinerary
{
    public Task<(IBus bus, double eta)[]?> GetFastestBus(IPosition from, IPosition to)
    {
        var pos = new PositionLL()
        {
            Latitude = 0,
            Longitude = 0
        };
        
        return Task.FromResult(new (IBus bus, double eta)[]
        {
            (new Bus()
            {
                Id = "im a unique bus",
                Name = "im still a unique bus",
                Position = pos,
                TransitTrip = new TransitTrip()
                {
                    Id = "im a unique trip",
                    RelevantDestination = new TransitStopSchedule()
                    {
                        Index = 0,
                        Stop = new Stop()
                        {
                            Id = "im a unique stop",
                            Position = pos,
                            Message = "pretty sure i'm unique"
                        },
                        DepartureTime = DateTime.Today
                    },
                    RelevantOrigin = new TransitStopSchedule()
                    {
                        Index = 0,
                        Stop = new Stop()
                        {
                            Id = "no you're not",
                            Position = pos,
                            Message = "pretty sure you're not a unique stop"
                        },
                        DepartureTime = DateTime.Today
                    },
                    StopSchedules = new List<TransitStopSchedule>(),
                    FromStaticGtfs = false,
                    RelevantDestinationStopId = "the right one",
                    RelevantOriginStopId = "the unique one"
                },
                StopIndexAtComputationTime = 0
            }, 10.0)
        })!;
    }

    public Task PrefetchAndApplyTripUpdates()
    {
        return Task.CompletedTask;
    }
}