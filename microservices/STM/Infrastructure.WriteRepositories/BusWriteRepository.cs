using Application.WriteServices.ServiceInterfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.WriteRepositories;

public class BusWriteRepository : DbContext, IBusWriteRepository
{
    private readonly ILogger<BusWriteRepository> _logger;

    public DbSet<Bus> Buses { get; set; } = null!;

    public BusWriteRepository(DbContextOptions<BusWriteRepository> options, ILogger<BusWriteRepository> logger) : base(options)
    {
        _logger = logger;
    }

    public Bus Get(string id)
    {
        return Buses.Find(id) ?? throw new KeyNotFoundException();
    }

    public void Save(Bus aggregate)
    {
        var bus = Buses.Find(aggregate.Id);
        
        if (bus == null)
        {
            Buses.Add(aggregate);
        }
        else
        {
            Buses.Update(aggregate);
        }

        SaveChanges();
    }
}