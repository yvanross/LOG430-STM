using Application.CommandServices.ServiceInterfaces.Repositories;
using Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.WriteRepositories;

public class StopWriteRepository : WriteRepository<Stop>, IStopWriteRepository
{
    public StopWriteRepository(DbContextOptions<WriteRepository<Stop>> options, ILogger logger) : base(options, logger)
    {
    }
}