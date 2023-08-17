using Application.CommandServices.ServiceInterfaces;
using Application.Common.Interfaces.Policies;
using Microsoft.Extensions.Logging;
using RestSharp;
using STM.ExternalServiceProvider.Proto;
using System.Collections.Concurrent;
using CodedInputStream = Google.Protobuf.CodedInputStream;

namespace Infrastructure.ApiClients;

public class StmClient : IStmClient
{
    private readonly IBackOffRetryPolicy<StmClient> _backOffRetryPolicy;
    private readonly ILogger<StmClient> _logger;

    private static string ApiKey => Environment.GetEnvironmentVariable("API_KEY") ?? string.Empty;

    private static readonly RestClient _stmClient = new("https://api.stm.info/pub/od/gtfs-rt/ic/v2");

    //Caching of feed position to not get rate limited by the STM Api
    private static ConcurrentDictionary<string, (VehiclePosition VehiclePosition, DateTime DateTime)> _feedPositions = new();

    //is a disposable reference
    private Timer _timer;

    public StmClient(IBackOffRetryPolicy<StmClient> backOffRetryPolicy, ILogger<StmClient> logger)
    {
        _backOffRetryPolicy = backOffRetryPolicy;
        _logger = logger;

        _timer = new Timer(_ => Task.Run(ProduceFeedPositionsAsync).ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                _logger.LogError(t.Exception, "Error while updating feed positions");
            }
        }), null, 0, (int)TimeSpan.FromSeconds(10).TotalMilliseconds);
    }

    private async Task ProduceFeedPositionsAsync()
    {
        var positions = await UpdateFeedPositions();

        var vehiclePositionAndTimeUpdatedList = positions.ToList()
            .ConvertAll(p => new ValueTuple<VehiclePosition, DateTime>(p, DateTime.UtcNow));

        AddOrUpdateEntries(vehiclePositionAndTimeUpdatedList);

        RemoveOldEntries();

        void AddOrUpdateEntries(IEnumerable<(VehiclePosition, DateTime)> vehiclePositionAndTimeUpdatedList)
        {
            foreach (var vp in vehiclePositionAndTimeUpdatedList.Select(v => (VehiclePosition: v, v.Item1.Vehicle.Id)))
            {
                _feedPositions.AddOrUpdate(vp.Id, _ => vp.VehiclePosition, (_, _) => vp.VehiclePosition);
            }
        }

        void RemoveOldEntries()
        {
            foreach (var tuple in _feedPositions)
                if (tuple.Value.DateTime.Subtract(DateTime.UtcNow) > TimeSpan.FromHours(1))
                    _feedPositions.Remove(tuple.Key, out _);
        }
    }

    private async Task<IEnumerable<VehiclePosition>> UpdateFeedPositions()
    {
        return await _backOffRetryPolicy.ExecuteAsync(async () =>
        {
            var requestPosition = new RestRequest("vehiclePositions/");

            requestPosition.AddHeader("apikey", ApiKey);

            var responsePosition = await _stmClient.ExecuteAsync(requestPosition);

            var feed = new FeedMessage();

            feed.MergeFrom(new CodedInputStream(responsePosition.RawBytes));

            return feed.Entity.ToList().ConvertAll(x => x.Vehicle);
        });
    }

    public IEnumerable<VehiclePosition> RequestFeedPositions()
    {
        return _feedPositions.Select(fp => fp.Value.VehiclePosition).ToList();
    }

    public async Task<IEnumerable<TripUpdate>> RequestFeedTripUpdates()
    {
        return await _backOffRetryPolicy.ExecuteAsync(async () =>
        {
            var requestTripUpdates = new RestRequest("tripUpdates/");

            requestTripUpdates.AddHeader("apikey", ApiKey);

            var responseTripUpdate = await _stmClient.ExecuteAsync(requestTripUpdates);

            var feed = new FeedMessage();

            feed.MergeFrom(new CodedInputStream(responseTripUpdate.RawBytes));

            return feed.Entity.ToList().ConvertAll(x => x.TripUpdate);
        });
    }
}
