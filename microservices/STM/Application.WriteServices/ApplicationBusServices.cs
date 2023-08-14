using Application.CommandServices.Seedwork;
using Application.CommandServices.ServiceInterfaces.Repositories;
using Domain.Entities;

namespace Application.CommandServices;

public class ApplicationBusServices : CommandAggregateService<Bus, IBusWriteRepository>
{
    public ApplicationBusServices(IBusWriteRepository busWrite) : base(busWrite)
    {

    }
}