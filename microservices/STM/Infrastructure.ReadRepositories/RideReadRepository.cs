using Application.QueryServices.ServiceInterfaces.Repositories;
using Domain.Aggregates.Ride;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ReadRepositories;

public class RideReadRepository : ReadRepository<Ride>, IRideReadRepository
{
    public RideReadRepository(DbContextOptions options, ILogger logger) : base(options, logger)
    {
    }

    public Ride GetRideMatchingParameters(string ScheduledDepartureId, string ScheduledDestinationId, string BusId)
    {
        var ride = Aggregates.FirstOrDefault(ride => ride.Departure.Id == ScheduledDepartureId && ride.Destination.Id == ScheduledDestinationId && ride.BusId == BusId);

        if (ride == null)
            throw new KeyNotFoundException();

        return ride;
    }
}