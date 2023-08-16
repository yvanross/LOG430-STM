using Application.CommandServices.ServiceInterfaces.Repositories;
using Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.WriteRepositories;

public class BusWriteRepository : WriteRepository<Bus>, IBusWriteRepository
{
    public BusWriteRepository(DbContextOptions<BusWriteRepository> options, ILogger<BusWriteRepository> logger) : base(options, logger) { }
}