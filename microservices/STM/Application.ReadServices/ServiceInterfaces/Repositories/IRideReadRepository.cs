using Application.QueryServices.Seedwork;
using Domain.Aggregates.Ride;

namespace Application.QueryServices.ServiceInterfaces.Repositories;

public interface IRideReadRepository : IReadRepository<Ride>
{
    public Ride GetRideMatchingParameters(string ScheduledDepartureId, string ScheduledDestinationId, string BusId);
}