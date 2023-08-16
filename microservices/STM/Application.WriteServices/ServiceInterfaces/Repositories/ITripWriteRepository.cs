using Application.CommandServices.Seedwork;
using Domain.Aggregates;

namespace Application.CommandServices.ServiceInterfaces.Repositories;

public interface ITripWriteRepository : IWriteRepository<Trip>
{
}