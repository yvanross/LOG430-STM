using System.Collections.Immutable;
using Domain.Aggregates.Stop;
using Domain.Aggregates.Trip;

namespace Application.QueryServices.ServiceInterfaces;

public interface IApplicationTripService
{
    Task<ImmutableHashSet<Trip>> TimeRelevantTripsContainingSourceAndDestination(IEnumerable<Stop> possibleSources, IEnumerable<Stop> possibleDestinations);
}