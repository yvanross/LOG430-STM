using Application.ReadServices.Seedwork;
using Domain.Entities;

namespace Application.ReadServices.ServiceInterfaces.Repositories;

public interface IBusReadRepository : IReadRepository<Bus>
{
    IEnumerable<Bus> GetAllIdsMatchingTripsIds(IEnumerable<string> tripIds);
}