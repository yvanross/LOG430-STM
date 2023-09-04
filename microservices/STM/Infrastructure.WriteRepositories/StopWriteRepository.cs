using Application.CommandServices.Repositories;
using Domain.Aggregates.Stop;
using Microsoft.Extensions.Logging;

namespace Infrastructure.WriteRepositories;

public class StopWriteRepository : WriteRepository<Stop>, IStopWriteRepository
{
    public StopWriteRepository(AppWriteDbContext writeDbContext, ILogger<StopWriteRepository> logger) : base(
        writeDbContext, logger)
    {
    }
}