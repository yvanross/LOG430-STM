using Application.ReadServices.Seedwork;
using Domain.Aggregates;

namespace Application.ReadServices.ServiceInterfaces.Repositories;

public interface ITripReadRepository : IReadRepository<Trip>
{
    IEnumerable<Trip> GetTripsContainingStopsId(IEnumerable<string> stopIds);
}