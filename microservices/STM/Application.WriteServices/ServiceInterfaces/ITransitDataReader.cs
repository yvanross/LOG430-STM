using Application.Mapping.Interfaces.Wrappers;

namespace Application.CommandServices.ServiceInterfaces;

public interface ITransitDataReader : IDisposable
{
    Stack<IStopWrapper> Stops { get; }

    Stack<ITripWrapper> Trips { get; }
}