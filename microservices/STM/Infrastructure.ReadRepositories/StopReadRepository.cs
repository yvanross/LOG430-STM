using Application.ReadServices.Seedwork;
using Application.ReadServices.ServiceInterfaces.Repositories;
using Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ReadRepositories;

public class StopReadRepository : DbContext, IStopReadRepository
{
    private readonly ILogger<StopReadRepository> _logger;

    public DbSet<Stop> Stops { get; set; } = null!;

    public StopReadRepository(DbContextOptions<StopReadRepository> options, ILogger<StopReadRepository> logger) : base(options)
    {
        _logger = logger;
    }


    public IEnumerable<Stop> GetAll()
    {
        return Stops.ToList();
    }

    public Stop Get(string id)
    {
        return Stops.Find(id) ?? throw new KeyNotFoundException();
    }
}