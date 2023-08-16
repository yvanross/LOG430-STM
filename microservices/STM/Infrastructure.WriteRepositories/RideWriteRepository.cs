using Application.CommandServices.ServiceInterfaces.Repositories;
using Domain.Aggregates.Ride;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.WriteRepositories;

public class RideWriteRepository : WriteRepository<Ride>, IRideWriteRepository
{
    public RideWriteRepository(DbContextOptions<RideWriteRepository> options, ILogger<RideWriteRepository> logger) : base(options, logger) { }
}