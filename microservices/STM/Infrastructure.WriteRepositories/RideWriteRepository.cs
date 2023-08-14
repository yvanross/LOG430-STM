using Application.CommandServices.ServiceInterfaces.Repositories;
using Domain.Aggregates;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static System.Formats.Asn1.AsnWriter;

namespace Infrastructure.WriteRepositories;

public class RideWriteRepository : DbContext, IRideWriteRepository
{
    private readonly ILogger<RideWriteRepository> _logger;

    public DbSet<Ride> Rides { get; set; } = null!;

    public RideWriteRepository(DbContextOptions<RideWriteRepository> options, ILogger<RideWriteRepository> logger) : base(options)
    {
        _logger = logger;
    }

    public Ride Get(string id)
    {
        return Rides.Find(id) ?? throw new KeyNotFoundException();
    }

    public void Save(Ride aggregate)
    {
        var ride = Rides.Find(aggregate.Id);

        if (ride == null)
        {
            Rides.Add(aggregate);
        }
        else
        {
            Rides.Update(aggregate);
        }

        SaveChanges();
    }
}