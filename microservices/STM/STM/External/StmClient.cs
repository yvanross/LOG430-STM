using ApplicationLogic.Extensions;
using ApplicationLogic.Interfaces;
using Google.Protobuf;
using RestSharp;
using STM.ExternalServiceProvider.Proto;

namespace STM.External;

public class StmClient : IStmClient
{
    private string APIKey { get; set; } = @"l7ba104e682e6143ecacf7fadccb8e3979";

    private static RestClient _stmClient = new ("https://api.stm.info/pub/od/gtfs-rt/ic/v2");

    public async Task<IEnumerable<VehiclePosition>> RequestFeedPositions()
    {
        return await Try.WithConsequenceAsync(async () =>
        {
            var requestPosition = new RestRequest("vehiclePositions/");
            requestPosition.AddHeader("apikey", APIKey);
            var responsePosition = await _stmClient.ExecuteAsync(requestPosition);

            var feed = new FeedMessage();
            feed.MergeFrom(new CodedInputStream(responsePosition.RawBytes));

            return feed.Entity.ToList().ConvertAll(x => x.Vehicle);
        }, 
            retryCount: 10,
            onFailure: async (_, _) => await Task.Delay(250));
    }

    public async Task<IEnumerable<TripUpdate>> RequestFeedTripUpdates()
    {
        return await Try.WithConsequenceAsync(async () =>
        {
            var requestTripUpdates = new RestRequest("tripUpdates/");
            requestTripUpdates.AddHeader("apikey", APIKey);
            var responseTripUpdate = await _stmClient.ExecuteAsync(requestTripUpdates);

            var feed = new FeedMessage();
            feed.MergeFrom(new CodedInputStream(responseTripUpdate.RawBytes));

            return feed.Entity.ToList().ConvertAll(x => x.TripUpdate);
        },
            retryCount: 10,
            onFailure: async (_, _) => await Task.Delay(250));
    }
}
