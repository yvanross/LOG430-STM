using Domain.Aggregates.Ride;
using Domain.Aggregates;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.ReadRepositories;

public class AppReadDbContext : DbContext
{
    public AppReadDbContext(DbContextOptions<AppReadDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ScheduledStop>(
            b =>
            {
                b.HasKey(e => e.Id);
                b.Property(e => e.DepartureTime);
                b.Property(e => e.StopId);
            });

        modelBuilder.Entity<Stop>(
            b =>
            {
                b.HasKey(e => e.Id);
                b.Property(e => e.Position);
                //b.HasOne(e => e.Position);
            });

        modelBuilder.Entity<Ride>(
            b =>
            {
                b.HasKey(e => e.Id);
                b.Property(e => e.Destination);
                b.Property(e => e.Departure);
                b.Property(e => e.PreviousStop);
                b.Property(e => e.TripBegunTime);
                b.Property(e => e.BusId);
                b.Property(e => e.DepartureReachedTime);
                b.Property(e => e.ReachedDepartureStop);
            });

        modelBuilder.Entity<Trip>(
            b =>
            {
                b.HasKey(e => e.Id);
                b.Property(e => e.ScheduledStops);
                //b.HasMany(e => e.ScheduledStops);
            });

        modelBuilder.Entity<Bus>(
            b =>
            {
                b.HasKey(e => e.Id);
                b.Property(e => e.TripId);
                b.Property(e => e.CurrentStopIndex);
                b.Property(e => e.Name);
            });
    }
}