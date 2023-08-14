using Application.CommandServices.Seedwork;
using Application.CommandServices.ServiceInterfaces.Repositories;
using Domain.Aggregates;

namespace Application.CommandServices;

public class ApplicationRideServices: CommandAggregateService<Ride, IRideWriteRepository>
{ 
    
}