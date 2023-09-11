using Application.CommandServices.Interfaces;
using Domain.Aggregates.Stop;

namespace Application.CommandServices.Repositories;

public interface IStopWriteRepository : IWriteRepository<Stop>
{
}