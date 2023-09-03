using Domain.Aggregates.Trip;

namespace Application.CommandServices.Repositories;

public interface ITripWriteRepository : IWriteRepository<Trip>
{
    void Update(Trip trip);
}