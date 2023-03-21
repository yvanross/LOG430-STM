using System.Collections.Immutable;
using ApplicationLogic.Extensions;
using ApplicationLogic.Interfaces;
using Google.Protobuf;
using RestSharp;
using STM.ExternalServiceProvider.Proto;

namespace STM.External;

public class StmClient : IStmClient
{
    private static string ApiKey => Environment.GetEnvironmentVariable("API_KEY") ?? string.Empty;

    private static readonly RestClient _stmClient = new ("https://api.stm.info/pub/od/gtfs-rt/ic/v2");

    private static ImmutableDictionary<string, (VehiclePosition VehiclePosition, DateTime DateTime)> _feedPositions = 
        ImmutableDictionary<string, (VehiclePosition VehiclePosition, DateTime DateTime)>.Empty;

    static StmClient()
    {
        _ = BeginProducingFeedPositions();
    }

    public static async Task BeginProducingFeedPositions()
    {
        _ = await Try.WithConsequenceAsync<Task>(async () =>
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
        }, retryCount:int.MaxValue);

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

    private static async Task<IEnumerable<VehiclePosition>> UpdateFeedPositions()
    {
        return await Try.WithConsequenceAsync(async () =>
        {
            var requestPosition = new RestRequest("vehiclePositions/");
            requestPosition.AddHeader("apikey", ApiKey);
            var responsePosition = await _stmClient.ExecuteAsync(requestPosition);

            var feed = new FeedMessage();
            feed.MergeFrom(new CodedInputStream(responsePosition.RawBytes));

            return feed.Entity.ToList().ConvertAll(x => x.Vehicle);
        }, 
            retryCount: 10,
            onFailure: async (_, _) => await Task.Delay(250));
    }

    public IEnumerable<VehiclePosition> RequestFeedPositions()
    {
        return _feedPositions.Select(fp=>fp.Value.VehiclePosition).ToList();
    }

    public async Task<IEnumerable<TripUpdate>> RequestFeedTripUpdates()
    {
        return await Try.WithConsequenceAsync(async () =>
        {
            var requestTripUpdates = new RestRequest("tripUpdates/");
            requestTripUpdates.AddHeader("apikey", ApiKey);
            var responseTripUpdate = await _stmClient.ExecuteAsync(requestTripUpdates);

            var feed = new FeedMessage();
            feed.MergeFrom(new CodedInputStream(responseTripUpdate.RawBytes));

            return feed.Entity.ToList().ConvertAll(x => x.TripUpdate);
        },
            retryCount: 10,
            onFailure: async (_, _) => await Task.Delay(250));
    }
}
