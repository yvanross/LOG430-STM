using Entities.Common.Concretions;
using Entities.Transit.Concretions;
using Entities.Transit.Interfaces;
using STM.ExternalServiceProvider.Proto;

namespace ApplicationLogic.Services.Itinerary;

public class GtfsProcessorService
{
    public IEnumerable<(IBus bus, double eta)> GetTimeRelevantBuses(IEnumerable<IBus> buses)
    {
        foreach (var bus in buses)
        {
            if (bus.TransitTrip.RelevantOrigin?.DepartureTime > DateTime.UtcNow &&
                bus.TransitTrip.RelevantDestination?.DepartureTime > DateTime.UtcNow &&
                bus.TransitTrip.RelevantOrigin?.DepartureTime < bus.TransitTrip.RelevantDestination?.DepartureTime)
            {
                yield return new(bus, (bus.TransitTrip.RelevantOrigin.Value.DepartureTime - DateTime.UtcNow).TotalSeconds);
            }
        }
    }

    public List<IBus> GetVehicleOnRelevantTrips(Dictionary<string, ITransitTrip> relevantTrips, IEnumerable<VehiclePosition> feedPositions)
    {
        var buses = new List<IBus>();

        var vehiclesAndTrips = feedPositions
            .Where(vehiclePosition => relevantTrips.ContainsKey(vehiclePosition.Trip.TripId))
            .Select(vehiclePosition =>
                (VehiclePosition: vehiclePosition,
                Trip: relevantTrips[vehiclePosition.Trip.TripId]))
            .ToList();

        foreach (var vehicle in vehiclesAndTrips.Select(x => x.VehiclePosition))
        {
            var bus = ConvertGtfsToFriendlyClass(vehicle, vehiclesAndTrips.Select(x => x.Trip).ToDictionary(x => x.Id));

            buses.Add(bus);
        }

        return buses;
    }

    public List<IBus> GetRelevantOriginAndDestinationForRelevantBuses(List<IBus> relevantBuses)
    {
        var filteredBuses = new List<IBus>();

        foreach (var bus in relevantBuses)
        {
            AssignRelevantOriginAndDestination(bus);

            if (bus.TransitTrip.RelevantOrigin is not null && bus.TransitTrip.RelevantDestination is not null)
            {
                filteredBuses.Add(bus);
            }
        }

        return filteredBuses;

        void AssignRelevantOriginAndDestination(IBus bus)
        {
            for (var index = 0; index < bus.TransitTrip.StopSchedules.Count; index++)
            {
                var stopSchedule = bus.TransitTrip.StopSchedules[index];

                if (bus.TransitTrip.RelevantOrigin is null && stopSchedule.Stop.Id.Equals(bus.TransitTrip.RelevantOriginStopId))
                {
                    bus.TransitTrip.RelevantOrigin = stopSchedule;
                    bus.TransitTrip.RelevantOriginStopId = stopSchedule.Stop.Id;
                }
                else if (bus.TransitTrip.RelevantOrigin is not null && bus.TransitTrip.RelevantDestination is null &&
                         stopSchedule.Stop.Id.Equals(bus.TransitTrip.RelevantDestinationStopId))
                {
                    bus.TransitTrip.RelevantDestination = stopSchedule;
                    bus.TransitTrip.RelevantDestinationStopId = stopSchedule.Stop.Id;
                }
            }
        }
    }

    private IBus ConvertGtfsToFriendlyClass(VehiclePosition vehicle, Dictionary<string, ITransitTrip> relevantTrips)
    {
        var bus = CreateBus(vehicle);

        var relevantTrip = relevantTrips[vehicle.Trip.TripId];

        bus.TransitTrip = CreateTrip(relevantTrip);

        return bus;
    }

    private static TransitTrip CreateTrip(ITransitTrip relevantTransitTrip)
    {
        int index = 0;

        return new TransitTrip()
        {
            Id = relevantTransitTrip.Id,

            StopSchedules = relevantTransitTrip.StopSchedules.ConvertAll(t =>
            {
                TransitStopSchedule transitStopSchedule = new TransitStopSchedule
                {
                    DepartureTime = t.DepartureTime,
                    Index = index,
                    Stop = new Stop()
                    {
                        Id = t.Stop.Id,
                        Position = t.Stop.Position,
                    }
                };

                index++;

                return transitStopSchedule;
            }),

            RelevantOriginStopId = relevantTransitTrip.RelevantOriginStopId,
            RelevantDestinationStopId = relevantTransitTrip.RelevantDestinationStopId
        };
    }

    private static Bus CreateBus(VehiclePosition vehicle)
    {
        return new Bus()
        {
            Id = vehicle.Vehicle.Id,
            Name = vehicle.Trip.RouteId,
            StopIndexAtComputationTime = (int)vehicle.CurrentStopSequence,
            Position = new PositionLL()
            {
                Longitude = vehicle.Position.Longitude,
                Latitude = vehicle.Position.Latitude
            }

        };
    }
}