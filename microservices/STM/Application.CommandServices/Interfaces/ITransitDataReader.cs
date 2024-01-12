using Application.Mapping.Interfaces.Wrappers;

namespace Application.CommandServices.Interfaces;

public interface ITransitDataReader : IDisposable
{
    IAsyncEnumerable<IStopWrapper> FetchStopData();

    IAsyncEnumerable<ITripWrapper> FetchTripData();
}