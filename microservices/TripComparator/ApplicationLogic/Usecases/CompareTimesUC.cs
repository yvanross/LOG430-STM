using System.Threading.Channels;
using ApplicationLogic.Interfaces;
using Entities.BusinessObjects;
using Entities.DomainInterfaces;
using TripComparator.DTO;

namespace ApplicationLogic.Usecases
{
    public class CompareTimesUC
    {
        private readonly IRouteTimeProvider _routeTimeProvider;

        private readonly IBusInfoProvider _iBusInfoProvider;

        private readonly IDataStreamWriteModel _dataStreamWriteModel;

        private readonly PeriodicTimer _periodicTimer = new(TimeSpan.FromMilliseconds(5));

        public CompareTimesUC(IRouteTimeProvider routeTimeProvider, IBusInfoProvider iBusInfoProvider, IDataStreamWriteModel dataStreamWriteModel)
        {
            _routeTimeProvider = routeTimeProvider;
            _iBusInfoProvider = iBusInfoProvider;
            _dataStreamWriteModel = dataStreamWriteModel;
        }

        public async Task<ChannelReader<ISaga>> BeginComparingBusAndCarTime(string startingCoordinates, string destinationCoordinates)
        {
            int averageCarTravelTime = 0;
            IStmBus? optimalBus = default;

            await Task.WhenAll(
                _routeTimeProvider.GetTravelTimeInSeconds(startingCoordinates, destinationCoordinates)
                    .ContinueWith(task => averageCarTravelTime = task.Result),
                _iBusInfoProvider.GetBestBus(startingCoordinates, destinationCoordinates)
                    .ContinueWith(task =>
                    {
                        optimalBus = task.Result.First();
                        return _iBusInfoProvider.BeginTracking(optimalBus);
                    }));

            if (optimalBus is null || averageCarTravelTime < 1)
            {
                throw new Exception("bus or car data was null");
            }

            var channel = Channel.CreateUnbounded<ISaga>();

            _ = Task.Run(async () =>
            {
                var trackingOnGoing = true;

                while (trackingOnGoing && await _periodicTimer.WaitForNextTickAsync())
                {
                    var trackingResult = await _iBusInfoProvider.GetTrackingUpdate(optimalBus.BusId);

                    if (trackingResult is null) continue;

                    trackingOnGoing = !trackingResult.TrackingCompleted;

                    var saga = new Saga()
                    {
                        Message = trackingResult.Message + $", by car it takes {averageCarTravelTime}",
                        Seconds = Convert.ToInt32(trackingResult.Duration),
                    };

                    await channel.Writer.WriteAsync(saga);
                }
                
                channel.Writer.Complete();
            });

            return channel.Reader;
        }

        public async Task WriteToStream(ChannelReader<ISaga> channelReader)
        {
            try
            {
                await _dataStreamWriteModel.BeginStreaming();

                await foreach (var saga in channelReader!.ReadAllAsync())
                {
                    await _dataStreamWriteModel.Produce(saga);
                }
            }
            finally
            {
                _dataStreamWriteModel.CloseChannel();
            }
        }
    }
}
