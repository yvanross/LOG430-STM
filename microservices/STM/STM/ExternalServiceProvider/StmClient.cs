using System.Globalization;
using System.Text;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Microsoft.AspNetCore.Mvc.Filters;
using ProtoBuf;
using ProtoBuf.Meta;
using RestSharp;
using STM.ExternalServiceProvider.Proto;
using DataFormat = ProtoBuf.DataFormat;

namespace STM.ExternalServiceProvider;

public class StmClient
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
