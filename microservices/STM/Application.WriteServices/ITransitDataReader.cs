using Application.Mapping.Interfaces.Wrappers;

namespace Application.CommandServices;

public interface ITransitDataReader : IDisposable
{
    Stack<IStopWrapper> Stops { get; }

    Stack<ITripWrapper> Trips { get; }

    void LoadStacks();
}