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


    public List<IBus> GetVehiculeOnRelevantTrips(ITripSTM[] relevantTrips, IEnumerable<VehiclePosition> feedPositions, ImmutableDictionary<string, TripUpdate> feedTripUpdates)
    {
        List<IBus> buses = new List<IBus>();

        var vehiculesOnTargetTrip = feedPositions.Where(v => relevantTrips.Any(t => t.Id.Equals(v.Trip.TripId)));

        var relevantTripsDictionary = relevantTrips.Where(relevantTrip =>
            vehiculesOnTargetTrip.Any(relevantVehicule => relevantVehicule.Trip.TripId.Equals(relevantTrip.Id))).DistinctBy(x=>x.Id).ToDictionary(relevantTrip => relevantTrip.Id);

        foreach (var vehicle in vehiculesOnTargetTrip)
        {
            var bus = new Bus()
            {
                currentStopIndex = (int)vehicle.CurrentStopSequence
            };

            if (TryConvertGTFSToFriendlyClass(vehicle, relevantTripsDictionary, feedTripUpdates, bus))
            {
                buses.Add(bus);
            }
        }
        return buses;
    }

    private bool TryConvertGTFSToFriendlyClass(VehiclePosition vehicle, Dictionary<string, ITripSTM> relevantTrips, IDictionary<string, TripUpdate> tripUpdates, IBus bus)
    {
        bus.Position = new PositionLL()
        {
            Longitude = vehicle.Position.Longitude,
            Latitude = vehicle.Position.Latitude
        };

        bus.Id = vehicle.Vehicle.Id;
        bus.Name = vehicle.Trip.RouteId;

        tripUpdates.TryGetValue(vehicle.Trip.TripId, out var updatedTrip);

        var updatedStopTimes = updatedTrip?.StopTimeUpdate.ToImmutableDictionary(t => t?.StopId ?? string.Empty);

        Func<IStopSTM,(DateTime? updatedStopTime, string message)?> GetDepartureTime = (IStopSTM t) =>
        {
            if (updatedStopTimes != null)
            {
                updatedStopTimes.TryGetValue(t.Id, out var stopTimeUpdate);

                if (stopTimeUpdate != null)
                {
                    var tuple = this.GetDepartureTime(stopTimeUpdate);

                    return tuple;
                } 
            }

            return null;
        };

        var tripToCopy = relevantTrips[vehicle.Trip.TripId];

        int index = 0;

        bus.Trip = new TripSTM()
        {
            Id = tripToCopy.Id,
            StopSchedules = tripToCopy.StopSchedules.ConvertAll(t =>
            {
                StopScheduleSTM stopScheduleStm = new StopScheduleSTM();

                var tuple = GetDepartureTime((IStopSTM)t.Stop);

                if (tuple?.updatedStopTime is not null)
                {
                    stopScheduleStm.DepartureTime = tuple.Value.updatedStopTime.Value;
                }

                stopScheduleStm.Index = index;

                stopScheduleStm.Stop = new StopSTM()
                {
                    Id = t.Stop.Id,
                    Position = t.Stop.Position,
                    Message = tuple?.message ?? "state is empty, probably using static time"
                };

                index++;

                return stopScheduleStm;
            }),
            
            RelevantOriginStopId = tripToCopy.RelevantOriginStopId,
            RelevantDestinationStopId = tripToCopy.RelevantDestinationStopId
        };

        bus.currentStopIndex = tripToCopy.FromStaticGtfs ? bus.currentStopIndex : 0;

        return true;
    }

    private (DateTime? scheduledStopTime, string stopState) GetDepartureTime(TripUpdate.Types.StopTimeUpdate stopTimeUpdate)
    {
        var time = stopTimeUpdate?.Departure?.Time ?? 0;

        if (time < 10000)
            time = stopTimeUpdate?.Arrival?.Time ?? 0;

        DateTime? dateTime = null;

        if (time > 10000)
            dateTime = DateTimeOffset.FromUnixTimeSeconds(time).UtcDateTime;

        return new(dateTime, stopTimeUpdate.ScheduleRelationship.ToString());
    }

    public List<IBus> GetRelevantOriginAndDestinationForRelevantBuses(List<IBus> relevantBuses)
    {
        List<IBus> filteredBuses = new List<IBus>();

        foreach (var bus in relevantBuses)
        {
            for (var index = 0; index < bus.Trip.StopSchedules.Count; index++)
            {
                var stopSchedule = bus.Trip.StopSchedules[index];
                
                if (bus.Trip.RelevantOrigin is null && stopSchedule.Stop.Id.Equals(bus.Trip.RelevantOriginStopId))
                {
                    var timeValidStop = RecursiveValidStopFinder(bus.Trip.StopSchedules, index);

                    if (timeValidStop is not null)
                    {
                        var stop = (StopScheduleSTM)timeValidStop?.stop;

                        stop.Index = timeValidStop.Value.index;

                        bus.Trip.RelevantOrigin = stop;
                        bus.Trip.RelevantOriginStopId = timeValidStop?.stop.Stop.Id;
                    }
                    else
                    {
                        break;
                    }
                }
                else if (bus.Trip.RelevantOrigin is not null && bus.Trip.RelevantDestination is null && stopSchedule.Stop.Id.Equals(bus.Trip.RelevantDestinationStopId))
                {
                    var timeValidStop = RecursiveValidStopFinder(bus.Trip.StopSchedules, index);

                    if (timeValidStop is not null)
                    {
                        var stop = (StopScheduleSTM)timeValidStop?.stop;

                        stop.Index = timeValidStop.Value.index;

                        bus.Trip.RelevantDestination = stop;
                        bus.Trip.RelevantDestinationStopId = timeValidStop?.stop.Stop.Id;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (bus.Trip.RelevantOrigin is not null && bus.Trip.RelevantDestination is not null)
            {
                filteredBuses.Add(bus);
            }
        }

        return filteredBuses;
    }

    private (IStopSchedule stop, int depth, int index)? RecursiveValidStopFinder(IList<StopScheduleSTM> stopSchedules, int index, int depth = 0)
    {
        const int MaxDepth = 4;

        if (depth < MaxDepth && index >= 0 && index < stopSchedules.Count &&
            stopSchedules[index] is IStopSchedule iStopSchedule)
        {
            if (iStopSchedule.DepartureTime < DateTime.UtcNow)
            {
                var earlierStop = RecursiveValidStopFinder(stopSchedules, --index, ++depth);
                var laterStop = RecursiveValidStopFinder(stopSchedules, ++index, ++depth);

                var best = (earlierStop?.depth ?? int.MaxValue) < (laterStop?.depth ?? int.MaxValue) ? earlierStop : laterStop;

                if ((best?.depth ?? int.MaxValue) < int.MaxValue)
                {
                    ((StopSTM)best.Value.stop.Stop).Message += $", this is not the originally intended stop, it is {best.Value.depth} before or after it";

                    return best;
                }

                return null;
            }
            
            return new(stopSchedules[index], depth, index);
        }

        return null;
    }
}