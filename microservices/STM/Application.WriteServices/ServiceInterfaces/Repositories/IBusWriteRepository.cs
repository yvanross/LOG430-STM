using Domain.Aggregates.Bus;

namespace Application.CommandServices.ServiceInterfaces.Repositories;

public interface IBusWriteRepository : IWriteRepository<Bus>
{
}