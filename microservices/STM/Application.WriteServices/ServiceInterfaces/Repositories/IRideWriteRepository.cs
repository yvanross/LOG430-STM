using Application.CommandServices.Seedwork;
using Domain.Aggregates.Ride;

namespace Application.CommandServices.ServiceInterfaces.Repositories;

public interface IRideWriteRepository : IWriteRepository<Ride>
{

}