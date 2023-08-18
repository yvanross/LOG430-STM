using Application.CommandServices.ServiceInterfaces.Repositories;
using Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.WriteRepositories;

public class BusWriteRepository : WriteRepository<Bus>, IBusWriteRepository
{
    public BusWriteRepository(DbContextOptions<WriteRepository<Bus>> options, ILogger logger) : base(options, logger)
    {
    }
}