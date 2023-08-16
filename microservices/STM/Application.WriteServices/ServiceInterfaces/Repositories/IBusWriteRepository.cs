using Application.CommandServices.Seedwork;
using Domain.Aggregates;

namespace Application.CommandServices.ServiceInterfaces.Repositories;

public interface IBusWriteRepository : IWriteRepository<Bus>
{
}