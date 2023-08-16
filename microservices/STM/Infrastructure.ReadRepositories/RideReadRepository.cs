using Domain.Aggregates.Ride;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ReadRepositories;

public class RideReadRepository : ReadRepository<Ride>
{
    public RideReadRepository(DbContextOptions options, ILogger logger) : base(options, logger)
    {
    }
}