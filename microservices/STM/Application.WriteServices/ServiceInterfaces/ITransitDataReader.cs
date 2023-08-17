using System.Collections.Immutable;
using Application.Mapping.Interfaces.Wrappers;

namespace Application.CommandServices.ServiceInterfaces;

public interface ITransitDataReader : IDisposable
{
    ImmutableList<IStopWrapper> Stops { get; }

    Lazy<ImmutableList<ITripWrapper>> Trips { get; }
}