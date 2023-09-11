using Application.CommandServices.Interfaces;
using Domain.Aggregates.Ride;

namespace Application.CommandServices.Repositories;

public interface IRideWriteRepository : IWriteRepository<Ride>
{ }