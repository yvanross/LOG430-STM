using Application.QueryServices.ServiceInterfaces.Repositories;
using Domain.Aggregates.Ride;

namespace Infrastructure.ReadRepositories;

public class RideReadRepository : ReadRepository<Ride>, IRideReadRepository
{
    public RideReadRepository(AppReadDbContext context) : base(context)
    {
    }

    public Ride GetRideMatchingParameters(string ScheduledDepartureId, string ScheduledDestinationId, string BusId)
    {
        var ride = Aggregates.FirstOrDefault(ride => ride.DepartureId.SequenceEqual(ScheduledDepartureId) && ride.DestinationId.SequenceEqual(ScheduledDestinationId) && ride.BusId.SequenceEqual(BusId));

        if (ride == null)
            throw new KeyNotFoundException();

        return ride;
    }
}