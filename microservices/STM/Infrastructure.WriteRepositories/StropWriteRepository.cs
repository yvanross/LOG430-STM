using Application.CommandServices.ServiceInterfaces.Repositories;
using Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.WriteRepositories;

public class StropWriteRepository : WriteRepository<Stop>, IStropWriteRepository
{
    public StropWriteRepository(DbContextOptions<StropWriteRepository> options, ILogger<StropWriteRepository> logger) : base(options, logger) { }
}