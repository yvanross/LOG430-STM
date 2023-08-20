using Domain.Aggregates.Trip;

namespace Application.CommandServices.ServiceInterfaces.Repositories;

public interface ITripWriteRepository : IWriteRepository<Trip>
{
}