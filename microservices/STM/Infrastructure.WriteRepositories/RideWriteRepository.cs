using Application.CommandServices.Repositories;
using Domain.Aggregates.Ride;
using Microsoft.Extensions.Logging;

namespace Infrastructure.WriteRepositories;

public class RideWriteRepository : WriteRepository<Ride>, IRideWriteRepository
{
    public RideWriteRepository(AppWriteDbContext writeDbContext, ILogger<RideWriteRepository> logger) : base(
        writeDbContext, logger)
    {
    }
}