using System.Collections.Concurrent;
using System.Net;
using Application.CommandServices.Interfaces;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Policies;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using RestSharp;
using STM.ExternalServiceProvider.Proto;

namespace Infrastructure.ApiClients;

public class StmClient : IStmClient
{
    private static readonly RestClient Client = new("https://api.stm.info/pub/od/gtfs-rt/ic/v2");

    //Caching of feed position to not get rate limited by the STM Api
    private static readonly ConcurrentDictionary<string, (VehiclePosition VehiclePosition, DateTime DateTime)> _feedPositions = new();

    //is a disposable reference
    private static Timer? _timer;
    private readonly IBackOffRetryPolicy<StmClient> _backOffRetryPolicy;
    private readonly IHostInfo _hostInfo;
    private readonly ILogger<StmClient> _logger;

    public StmClient(IBackOffRetryPolicy<StmClient> backOffRetryPolicy, IHostInfo hostInfo, ILogger<StmClient> logger)
    {
        _backOffRetryPolicy = backOffRetryPolicy;
        _hostInfo = hostInfo;
        _logger = logger;

        _timer ??= new Timer(_ => Task.Run(ProduceFeedPositionsAsync).ContinueWith(t =>
        {
            if (t.IsFaulted) _logger.LogError(t.Exception, "Error while updating feed positions");
        }), null, 0, (int)TimeSpan.FromSeconds(10).TotalMilliseconds);
    }

    public IEnumerable<VehiclePosition> RequestFeedPositions()
    {
        return _feedPositions.Select(fp => fp.Value.VehiclePosition).ToList();
    }

    public async Task<IEnumerable<TripUpdate>> RequestFeedTripUpdates()
    {
        return await _backOffRetryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                var requestTripUpdates = new RestRequest("tripUpdates/");

                requestTripUpdates.AddHeader("apikey", _hostInfo.GetStmApiKey());

                var responseTripUpdate = await Client.ExecuteAsync(requestTripUpdates);

                IsRateLimited(responseTripUpdate);

                var feed = new FeedMessage();

                feed.MergeFrom(new CodedInputStream(responseTripUpdate.RawBytes));

                return feed.Entity.ToList().ConvertAll(x => x.TripUpdate).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while requesting feed trip updates");

                throw;
            }
            
        });
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
                _feedPositions.AddOrUpdate(vp.Id, _ => vp.VehiclePosition, (_, _) => vp.VehiclePosition);
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
        try
        {
            var requestPosition = new RestRequest("vehiclePositions/");

            requestPosition.AddHeader("apikey", _hostInfo.GetStmApiKey());

            var responsePosition = await Client.ExecuteAsync(requestPosition);

            IsRateLimited(responsePosition);

            var feed = new FeedMessage();

            feed.MergeFrom(new CodedInputStream(responsePosition.RawBytes));

            return feed.Entity.ToList().ConvertAll(x => x.Vehicle);
        }
        catch (InvalidProtocolBufferException)
        {
            _logger.LogError(
                """
                        Error while parsing STM feed
                        This is sometimes due to a wrong API key
                        Validate your key by testing it on 
                        https://portail.developpeurs.stm.info/apihub/?_gl=1*nsvvxk*_ga*MTA1MTIyMTQ0Mi4xNjc2MDU0OTc3*_ga_37MDMXFX83*MTY5MzkxNzMzNC4yMS4xLjE2OTM5MTc0MjYuNDAuMC4w#/apis/bc64b63f-4ef4-4055-bd2f-eb3bf30ddd16/show/spec
                        """);
        }
       
        return Enumerable.Empty<VehiclePosition>();
    }

    private void IsRateLimited(RestResponse response)
    {
        if (response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.TooManyRequests)
        {
            _logger.LogError(response.ErrorMessage);

            throw new Exception(response.ErrorMessage);
        }
    }
}