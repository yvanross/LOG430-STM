using Application.WriteServices.ServiceInterfaces.Repositories;
using Domain.Aggregates;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.WriteRepositories;

public class StropWriteRepository : DbContext, IStropWriteRepository
{
    private readonly ILogger<StropWriteRepository> _logger;

    public DbSet<Stop> Stops { get; set; } = null!;

    public StropWriteRepository(DbContextOptions<StropWriteRepository> options, ILogger<StropWriteRepository> logger) : base(options)
    {
        _logger = logger;
    }

    public Stop Get(string id)
    {
        return Stops.Find(id) ?? throw new KeyNotFoundException();
    }

    public void Save(Stop aggregate)
    {
        var stop = Stops.Find(aggregate.Id);

        if (stop == null)
        {
            Stops.Add(aggregate);
        }
        else
        {
            Stops.Update(aggregate);
        }

        SaveChanges();
    }
}