using Application.Mapping.Interfaces.Wrappers;

namespace Application.CommandServices.Interfaces;

public interface ITransitDataReader : IDisposable
{
    void LoadStaticGtfsFromFilesInMemory();

    IEnumerable<IStopWrapper> FetchStopData();

    IEnumerable<ITripWrapper> FetchTripData();
}