﻿using Application.ReadServices.ServiceInterfaces.Repositories;
using Application.ViewModels;
using Domain.Aggregates;
using Application.Common.Extensions;
using Application.Common.Exceptions;

namespace Application.ReadServices;

public class ApplicationBusServices
{
    private readonly IBusReadRepository _busRead;

    public ApplicationBusServices(IBusReadRepository busRead)
    {
        _busRead = busRead;
    }

    public IEnumerable<RideViewModel> GetTimeRelevantRideViewModels(Dictionary<string, Trip> trips, Dictionary<string, Stop> sources, Dictionary<string, Stop> destination)
    {
        var buses = _busRead.GetAllIdsMatchingTripsIds(trips.Keys);

        var rideViewModels = buses
            .Select(bus => 
                new RideViewModel(
                trips[bus.TripId].FirstMatchingStop(sources).Id,
                trips[bus.TripId].LastMatchingStop(destination).Id,
                bus.Id,
                trips[bus.TripId].Id))
            .OrderBy(ride => trips[ride.TripId].GetStopDepartureTime(ride.ScheduledDepartureId))
            .ToList();

        if (rideViewModels.IsEmpty()) throw new NoBusesFoundException();

        return rideViewModels;
    }
}