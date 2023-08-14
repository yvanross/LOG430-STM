using Application.CommandServices.Seedwork;
using Application.CommandServices.ServiceInterfaces.Repositories;
using Domain.Aggregates;

namespace Application.CommandServices;

public class ApplicationStopService : CommandAggregateService<Stop, IStropWriteRepository>
{
    public ApplicationStopService(IStropWriteRepository readStops) : base(readStops)
    {
    }
}