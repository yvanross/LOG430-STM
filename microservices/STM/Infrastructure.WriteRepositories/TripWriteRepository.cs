using Application.CommandServices.ServiceInterfaces.Repositories;
using Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.WriteRepositories;

public class TripWriteRepository : WriteRepository<Trip>, ITripWriteRepository
{
    public TripWriteRepository(DbContextOptions<WriteRepository<Trip>> options, ILogger logger) : base(options, logger)
    {
    }
}