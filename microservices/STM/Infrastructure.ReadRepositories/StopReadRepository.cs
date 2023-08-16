using Application.QueryServices.ServiceInterfaces.Repositories;
using Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ReadRepositories;

public class StopReadRepository : ReadRepository<Stop>, IStopReadRepository
{
    public StopReadRepository(DbContextOptions options, ILogger logger) : base(options, logger)
    {
    }
}