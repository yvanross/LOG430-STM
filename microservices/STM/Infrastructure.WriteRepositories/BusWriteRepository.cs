using Application.CommandServices.Repositories;
using Domain.Aggregates.Bus;
using Microsoft.Extensions.Logging;

namespace Infrastructure.WriteRepositories;

public class BusWriteRepository : WriteRepository<Bus>, IBusWriteRepository
{
    public BusWriteRepository(AppWriteDbContext writeDbContext, ILogger<BusWriteRepository> logger) : base(
        writeDbContext, logger)
    {
    }
}