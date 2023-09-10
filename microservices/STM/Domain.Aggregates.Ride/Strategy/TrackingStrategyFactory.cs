using Domain.Common.Interfaces;

namespace Domain.Aggregates.Ride.Strategy;

internal static class TrackingStrategyFactory
{
    //Argument to use a finite state machine,
    //but it would be overkill for this simple case, especially since the state changes are only forward.
    //It would be more code duplication and object creation than it's worth
    internal static TrackingStrategy Create(RideUpdateInfo rideUpdateInfo, IDatetimeProvider datetimeProvider, DateTime tripBegunTime, DateTime? departureReachedTime)
    {
        return rideUpdateInfo.CurrentStopIndex switch
        {
            var n when BeforeFirstStop(n) => new BeforeDepartureTracking(datetimeProvider, tripBegunTime, rideUpdateInfo),
            var n when AtFirstStop(n) => new AtDepartureTracking(datetimeProvider, tripBegunTime, departureReachedTime, rideUpdateInfo),
            var n when OnRouteToDestination(n) => new AfterDepartureTracking(datetimeProvider, tripBegunTime, departureReachedTime, rideUpdateInfo),
            _ => new ReachedDestinationStop(datetimeProvider, tripBegunTime, departureReachedTime, rideUpdateInfo)
        };

        bool BeforeFirstStop(int n)
        {
            return n < rideUpdateInfo.FirstStopIndex;
        }

        bool AtFirstStop(int n)
        {
            return n == rideUpdateInfo.FirstStopIndex;
        }

        bool OnRouteToDestination(int n)
        {
            return n > rideUpdateInfo.FirstStopIndex && n < rideUpdateInfo.TargetStopIndex;
        }
    }
}