using Application.QueryServices.Seedwork;
using Domain.Aggregates.Trip;

namespace Application.QueryServices.ServiceInterfaces.Repositories;

public interface ITripReadRepository : IReadRepository<Trip>
{
    Task<IEnumerable<Trip>> GetTripsContainingStopsId(IEnumerable<string> stopIds);
}