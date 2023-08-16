using Application.QueryServices.Seedwork;
using Domain.Aggregates;

namespace Application.QueryServices.ServiceInterfaces.Repositories;

public interface ITripReadRepository : IReadRepository<Trip>
{
    IEnumerable<Trip> GetTripsContainingStopsId(IEnumerable<string> stopIds);
}