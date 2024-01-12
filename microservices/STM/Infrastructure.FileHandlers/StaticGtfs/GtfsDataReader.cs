using Application.CommandServices.Interfaces;
using Application.Mapping.Interfaces.Wrappers;
using Infrastructure.FileHandlers.StaticGtfs.Processor;

namespace Infrastructure.FileHandlers.StaticGtfs;

public sealed class GtfsDataReader(TripProcessor tripProcessor, StopsProcessor stopsProcessor) : ITransitDataReader
{
    public IAsyncEnumerable<IStopWrapper> FetchStopData()
    {
        return stopsProcessor.Process();
    }

    public IAsyncEnumerable<ITripWrapper> FetchTripData()
    {
        return tripProcessor.Process();
    }

    public void Dispose()
    {
        tripProcessor.Dispose();
        stopsProcessor.Dispose();

        GC.SuppressFinalize(this);
    }

    ~GtfsDataReader()
    {
        Dispose();
    }
}