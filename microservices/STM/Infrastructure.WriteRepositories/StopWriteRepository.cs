using Application.CommandServices.ServiceInterfaces.Repositories;
using Domain.Aggregates;
using Microsoft.Extensions.Logging;

namespace Infrastructure.WriteRepositories;

public class StopWriteRepository : WriteRepository<Stop>, IStopWriteRepository
{
    public StopWriteRepository(AppWriteDbContext writeDbContext, ILogger<StopWriteRepository> logger) : base(writeDbContext, logger)
    {
    }
}