using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationLogic.Interfaces;

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
                TransitTripId = new Ride()
                {
                    Id = "im a unique trip",
                    RelevantDestination = new IndexedStopSchedule()
                    {
                        Index = 0,
                        StopId = new Stop()
                        {
                            Id = "im a unique stop",
                            Position = pos,
                            Message = "pretty sure i'm unique"
                        },
                        DepartureTime = DateTime.Today
                    },
                    RelevantOrigin = new IndexedStopSchedule()
                    {
                        Index = 0,
                        StopId = new Stop()
                        {
                            Id = "no you're not",
                            Position = pos,
                            Message = "pretty sure you're not a unique stop"
                        },
                        DepartureTime = DateTime.Today
                    },
                    StopSchedules = new List<IndexedStopSchedule>(),
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