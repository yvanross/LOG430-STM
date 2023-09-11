using Application.CommandServices.Interfaces;
using Domain.Aggregates.Bus;

namespace Application.CommandServices.Repositories;

public interface IBusWriteRepository : IWriteRepository<Bus>
{
    void RemoveOldBuses(DateTime cutOffDate);
}