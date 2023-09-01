using Application.CommandServices.Repositories;
using Domain.Aggregates.Trip;
using Microsoft.Extensions.Logging;

namespace Infrastructure.WriteRepositories;

public class TripWriteRepository : WriteRepository<Trip>, ITripWriteRepository
{
    public TripWriteRepository(AppWriteDbContext writeDbContext, ILogger<TripWriteRepository> logger) : base(writeDbContext, logger)
    {
    }
}