using Domain.Aggregates.Bus;

namespace Application.QueryServices.ServiceInterfaces.Repositories;

public interface IBusReadRepository : IReadRepository<Bus>
{
    IEnumerable<Bus> GetAllIdsMatchingTripsIds(IEnumerable<string> tripIds);
}