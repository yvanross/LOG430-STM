using Application.ReadServices;
using Domain.Aggregates;
using Domain.Common.Interfaces;
using Domain.Entities;
using STM.ExternalServiceProvider.Proto;

namespace ApplicationLogic.Services.Itinerary;

public class GtfsProcessorService
{
    private readonly IDatetimeProvider _datetimeProvider;
    private readonly ApplicationBusServices _applicationBusServices;

    public GtfsProcessorService(IDatetimeProvider datetimeProvider, ApplicationBusServices applicationBusServices)
    {
        _datetimeProvider = datetimeProvider;
        _applicationBusServices = applicationBusServices;
    }

    public List<Ride> GetRidesFromPositions(Dictionary<string, Trip> relevantTrips)
    {
        var busDtosAndTrips = _applicationBusServices.GetRides(relevantTrips, feedPositions);
    }

}