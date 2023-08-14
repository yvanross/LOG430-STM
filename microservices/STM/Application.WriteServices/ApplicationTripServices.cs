using Application.CommandServices.Seedwork;
using Application.CommandServices.ServiceInterfaces.Repositories;
using Domain.Aggregates;

namespace Application.CommandServices;

public class ApplicationTripServices : CommandAggregateService<Trip, ITripWriteRepository>
{
    public ApplicationTripServices(ITripWriteRepository tripWrite) : base(tripWrite)
    {
        
    }
}