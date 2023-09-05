using Domain.Aggregates.Bus;
using Domain.Aggregates.Ride;
using Domain.Aggregates.Stop;
using Domain.Aggregates.Trip;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.WriteRepositories;

public sealed class AppWriteDbContext : DbContext
{
    public AppWriteDbContext(DbContextOptions<AppWriteDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Stop>(
            b =>
            {
                b.HasKey(e => e.Id);
                b.OwnsOne(e => e.Position);
            });

        modelBuilder.Entity<Ride>(
            b =>
            {
                b.HasKey(e => e.Id);
                b.Property(e => e.DestinationId);
                b.Property(e => e.DepartureId);
                b.Property(e => e.FirstRecordedStopId);
                b.Property(e => e.TripBegunTime);
                b.Property(e => e.BusId);
                b.Property(e => e.DepartureReachedTime);
                b.Property(e => e.ReachedDepartureStop);
            });

        modelBuilder.Entity<Bus>(
            b =>
            {
                b.HasKey(e => e.Id);
                b.Property(e => e.TripId);
                b.Property(e => e.CurrentStopIndex);
                b.Property(e => e.Name);
            });

        //needed since the in memory provider doesn't fare well with shadow properties
        if (this.IsInMemory())
        {
            modelBuilder.Entity<Trip>(
                b =>
                {
                    b.HasKey(e => e.Id);
                    b.HasMany(e => e.ScheduledStops)
                        .WithOne()
                        .HasForeignKey("TripId");
                });

            modelBuilder.Entity<ScheduledStop>(
                b =>
                {
                    b.HasKey(e => e.Id);
                    b.Property<string>("TripId");
                    b.Property(e => e.StopId);
                    b.Property(e => e.DepartureTime);
                });
        }
        else
        {
            modelBuilder.Entity<Trip>(
                b =>
                {
                    b.HasKey(e => e.Id);
                    b.OwnsMany(e => e.ScheduledStops);
                });
        }
    }
}