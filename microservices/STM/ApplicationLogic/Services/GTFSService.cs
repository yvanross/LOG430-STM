using System.Collections.Immutable;
using Entities.Concretions;
using Entities.Domain;
using STM.ExternalServiceProvider.Proto;

namespace ApplicationLogic.Services;

public class GTFSService
{
    public IEnumerable<(IBus bus, double eta)> GetTimeRelevantBuses(IEnumerable<IBus> buses)
    {
        foreach (var bus in buses)
        {
            if (bus.Trip.RelevantOrigin?.DepartureTime > DateTime.UtcNow && bus.Trip.RelevantDestination?.DepartureTime > DateTime.UtcNow && bus.Trip.RelevantOrigin?.DepartureTime < bus.Trip.RelevantDestination?.DepartureTime)
            {
                yield return new (bus, (bus.Trip.RelevantOrigin.Value.DepartureTime - DateTime.UtcNow).TotalSeconds);
            }
        }
    }

    public List<IBus> GetVehicleOnRelevantTrips(ImmutableDictionary<string, ITripSTM> relevantTrips, IEnumerable<VehiclePosition> feedPositions)
    {
        List<IBus> buses = new List<IBus>();

        var vehiclesAndTrips = feedPositions
            .Where(vehiclePosition => relevantTrips.ContainsKey(vehiclePosition.Trip.TripId))
            .Select(vehiclePosition => 
                (VehiclePosition: vehiclePosition,
                Trip: relevantTrips[vehiclePosition.Trip.TripId]))
            .ToList();

        foreach (var vehicle in vehiclesAndTrips.Select(x=>x.VehiclePosition))
        {
            var bus = ConvertGtfsToFriendlyClass(vehicle, vehiclesAndTrips.Select(x => x.Trip).ToDictionary(x => x.Id));

            buses.Add(bus);
        }

        return buses;
    }

    private IBus ConvertGtfsToFriendlyClass(VehiclePosition vehicle, Dictionary<string, ITripSTM> relevantTrips)
    {
        var bus = new Bus()
        {
            Id = vehicle.Vehicle.Id,
            Name = vehicle.Trip.RouteId,
            currentStopIndex = (int)vehicle.CurrentStopSequence,
            Position = new PositionLL()
            {
                Longitude = vehicle.Position.Longitude,
                Latitude = vehicle.Position.Latitude
            }

        };
        
        var relevantTrip = relevantTrips[vehicle.Trip.TripId];

        var index = 0;

        bus.Trip = new TripSTM()
        {
            Id = relevantTrip.Id,

            StopSchedules = relevantTrip.StopSchedules.ConvertAll(t =>
            {
                StopScheduleSTM stopScheduleStm = new StopScheduleSTM
                {
                    DepartureTime = t.DepartureTime,
                    Index = index,
                    Stop = new StopSTM()
                    {
                        Id = t.Stop.Id,
                        Position = t.Stop.Position,
                    }
                };

                index++;

                return stopScheduleStm;
            }),
            
            RelevantOriginStopId = relevantTrip.RelevantOriginStopId,
            RelevantDestinationStopId = relevantTrip.RelevantDestinationStopId
        };

        return bus;
    }

    public List<IBus> GetRelevantOriginAndDestinationForRelevantBuses(List<IBus> relevantBuses)
    {
        var filteredBuses = new List<IBus>();

        foreach (var bus in relevantBuses)
        {
            for (var index = 0; index < bus.Trip.StopSchedules.Count; index++)
            {
                var stopSchedule = bus.Trip.StopSchedules[index];
                
                if (bus.Trip.RelevantOrigin is null && stopSchedule.Stop.Id.Equals(bus.Trip.RelevantOriginStopId))
                {
                    bus.Trip.RelevantOrigin = stopSchedule;
                    bus.Trip.RelevantOriginStopId = stopSchedule.Stop.Id;
                }
                else if (bus.Trip.RelevantOrigin is not null && bus.Trip.RelevantDestination is null && stopSchedule.Stop.Id.Equals(bus.Trip.RelevantDestinationStopId))
                {
                    bus.Trip.RelevantDestination = stopSchedule;
                    bus.Trip.RelevantDestinationStopId = stopSchedule.Stop.Id;
                }
            }

            if (bus.Trip.RelevantOrigin is not null && bus.Trip.RelevantDestination is not null)
            {
                filteredBuses.Add(bus);
            }
        }

        return filteredBuses;
    }
}