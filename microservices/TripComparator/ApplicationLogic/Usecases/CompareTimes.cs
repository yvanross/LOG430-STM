using System.Threading.Channels;
using ApplicationLogic.Interfaces;
using Entities.BusinessObjects;
using Entities.DomainInterfaces;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Usecases
{
    public class CompareTimes
    {
        private readonly IRouteTimeProvider _routeTimeProvider;

        private readonly IBusInfoProvider _iBusInfoProvider;

        private readonly IDataStreamWriteModel _dataStreamWriteModel;

        private readonly PeriodicTimer _periodicTimer = new(TimeSpan.FromMilliseconds(50));

        private int _averageCarTravelTime;

        private IStmBus? _optimalBus;

        //Todo tactique modifiablité Injection de dépendance
        public CompareTimes(IRouteTimeProvider routeTimeProvider, IBusInfoProvider iBusInfoProvider, IDataStreamWriteModel dataStreamWriteModel, ILogger<CompareTimes> logger)
        {
            _routeTimeProvider = routeTimeProvider;
            _iBusInfoProvider = iBusInfoProvider;
            _dataStreamWriteModel = dataStreamWriteModel;
        }

        public async Task<Channel<IBusPositionUpdated>> BeginComparingBusAndCarTime(string startingCoordinates, string destinationCoordinates)
        {
            await Task.WhenAll(
                
                _routeTimeProvider.GetTravelTimeInSeconds(startingCoordinates, destinationCoordinates)
                    .ContinueWith(task => _averageCarTravelTime = task.Result),

                _iBusInfoProvider.GetBestBus(startingCoordinates, destinationCoordinates)
                    .ContinueWith(task =>
                    {
                        _optimalBus = task.Result.First();
                        return _iBusInfoProvider.BeginTracking(_optimalBus);
                    })
                );

            if (_optimalBus is null || _averageCarTravelTime < 1)
            {
                throw new Exception("bus or car data was null");
            }

            var channel = Channel.CreateUnbounded<IBusPositionUpdated>();

            return channel;
        }

        public async Task PollTrackingUpdate(ChannelWriter<IBusPositionUpdated> channel)
        {
            var trackingOnGoing = true;

            while (trackingOnGoing && await _periodicTimer.WaitForNextTickAsync())
            {
                var trackingResult = await _iBusInfoProvider.GetTrackingUpdate(_optimalBus!.BusId);

                if (trackingResult is null) continue;

                trackingOnGoing = !trackingResult.TrackingCompleted;

                var busPosition = new BusPosition()
                {
                    Message = trackingResult.Message + $"\nCar: {_averageCarTravelTime} seconds",
                    Seconds = Convert.ToInt32(trackingResult.Duration),
                };

                await channel.WriteAsync(busPosition);
            }

            channel.Complete();
        }

        public async Task WriteToStream(ChannelReader<IBusPositionUpdated> channelReader)
        {
            await foreach (var busPositionUpdated in channelReader!.ReadAllAsync())
            {
                await _dataStreamWriteModel.Produce(busPositionUpdated);
            }
        }
    }
}
