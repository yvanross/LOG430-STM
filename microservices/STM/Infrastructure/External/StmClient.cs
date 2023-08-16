using System.Collections.Immutable;
using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Policies;
using Google.Protobuf;
using RestSharp;
using STM.ExternalServiceProvider.Proto;

namespace Infrastructure2.External;

public class StmClient : IStmClient
{
    private readonly IBackOffRetryPolicy<StmClient> _backOffRetryPolicy;
    private readonly IInfiniteRetryPolicy<StmClient> _infiniteRetryPolicy;
    private static string ApiKey => Environment.GetEnvironmentVariable("API_KEY") ?? string.Empty;

    private static readonly RestClient _stmClient = new ("https://api.stm.info/pub/od/gtfs-rt/ic/v2");

    //todo Caching of feed position to not get rate limited by the STM Api
    //(not using concurrent dict here because imm. dict converts to a list faster while still being thread safe and lockless)
    private static ImmutableDictionary<string, (VehiclePosition VehiclePosition, DateTime DateTime)> _feedPositions = 
        ImmutableDictionary<string, (VehiclePosition VehiclePosition, DateTime DateTime)>.Empty;

    public StmClient(IBackOffRetryPolicy<StmClient> backOffRetryPolicy, IInfiniteRetryPolicy<StmClient> infiniteRetryPolicy)
    {
        _backOffRetryPolicy = backOffRetryPolicy;
        _infiniteRetryPolicy = infiniteRetryPolicy;

        _ = BeginProducingFeedPositions();
    }

    async Task BeginProducingFeedPositions()
    {
        await _infiniteRetryPolicy.ExecuteAsync(async () =>
        {
            while (true)
            {
                var positions = await UpdateFeedPositions();

                var vehiclePositionAndTimeUpdatedList = positions.ToList()
                    .ConvertAll(p => new ValueTuple<VehiclePosition, DateTime>(p, DateTime.UtcNow));

                AddOrUpdateEntries(vehiclePositionAndTimeUpdatedList);

                RemoveOldEntries();

                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        });

        void AddOrUpdateEntries(IEnumerable<(VehiclePosition, DateTime)> vehiclePositionAndTimeUpdatedList)
        {
            foreach (var vp in vehiclePositionAndTimeUpdatedList.Select(v => (VehiclePosition: v, v.Item1.Vehicle.Id)))
            {
                if (_feedPositions.ContainsKey(vp.Id))
                    _feedPositions = _feedPositions.Remove(vp.Id);

                _feedPositions = _feedPositions.Add(vp.Id, vp.VehiclePosition);
            }
        }

        void RemoveOldEntries()
        {
            foreach (var tuple in _feedPositions)
                if (tuple.Value.DateTime.Subtract(DateTime.UtcNow) > TimeSpan.FromHours(1))
                    _feedPositions = _feedPositions.Remove(tuple.Key);
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
        return _feedPositions.Select(fp=>fp.Value.VehiclePosition).ToList();
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
