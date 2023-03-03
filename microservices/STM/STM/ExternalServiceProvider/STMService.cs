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

public class STMService
{
    private string APIKey { get; set; } = @"l7xx1da0d6f58a654dc0a603494ebbec277a";

    public async Task<bool> CheckSTMAvailability()
    {
        var client = new RestClient("https://api.stm.info/pub/od/i3/v1/messages/etatservice/");
        var request = new RestRequest();
        request.AddHeader("apikey", APIKey);
        request.AddHeader("origin", "mon.domain.xyz");
        var response = client.Execute(request);

        return true;
    }

    public async Task<IEnumerable<VehiclePosition>> RequestFeedPositions()
    {
        var clientPosition = new RestClient("https://api.stm.info/pub/od/gtfs-rt/ic/v2/vehiclePositions/");
        var requestPosition = new RestRequest();
        requestPosition.AddHeader("apikey", APIKey);
        var responsePosition = await clientPosition.ExecuteAsync(requestPosition);

        FeedMessage feed = new FeedMessage();
        feed.MergeFrom(new CodedInputStream(responsePosition.RawBytes));

        return feed.Entity.ToList().ConvertAll(x => x.Vehicle);
    }

    public async Task<IEnumerable<TripUpdate>> RequestFeedTripUpdates()
    {
        var clientTripUpdate = new RestClient("https://api.stm.info/pub/od/gtfs-rt/ic/v2/tripUpdates/");
        var requestTripUpdates = new RestRequest();
        requestTripUpdates.AddHeader("apikey", APIKey);
        var responseTripUpdate = await clientTripUpdate.ExecuteAsync(requestTripUpdates);

        FeedMessage feed = new FeedMessage();
        feed.MergeFrom(new CodedInputStream(responseTripUpdate.RawBytes));

        return feed.Entity.ToList().ConvertAll(x=>x.TripUpdate);
    }
}
