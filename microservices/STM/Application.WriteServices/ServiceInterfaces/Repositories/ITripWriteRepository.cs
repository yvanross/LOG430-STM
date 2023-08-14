using Application.WriteServices.Seedwork;
using Domain.Aggregates;

namespace Application.WriteServices.ServiceInterfaces.Repositories;

public interface ITripWriteRepository : IWriteRepository<Trip>
{
}