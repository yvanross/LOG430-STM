using Application.Mapping.Interfaces.Wrappers;
using System.Collections.Immutable;

namespace Application.CommandServices.ServiceInterfaces;

public interface ITransitDataReader : IDisposable
{
    ImmutableList<IStopWrapper> Stops { get; }

    Lazy<ImmutableList<ITripWrapper>> Trips { get; }
}