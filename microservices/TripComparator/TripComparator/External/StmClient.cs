using System.Net;
using System.Runtime.Serialization;
using Ambassador;
using Ambassador.BusinessObjects;
using Ambassador.BusinessObjects.InterServiceRequests;
using Ambassador.Controllers;
using Entities.BusinessObjects;
using Entities.DomainInterfaces;
using Newtonsoft.Json;
using TripComparator.DTO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TripComparator.External;

public class StmClient : IBusInfoProvider
{
    readonly ILogger? _logger;

    public StmClient(ILogger? logger)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<IStmBus?>> GetBestBus(string startingCoordinates, string destinationCoordinates)
    {
        var channel = await RestController.Get(new GetRoutingRequest()
        {
            TargetService = "STM",
            Endpoint = $"Finder/OptimalBuses",
            Params = new List<NameValue>()
            {
                new ()
                {
                    Name = "fromLatitudeLongitude",
                    Value = startingCoordinates
                },
                new ()
                {
                    Name = "toLatitudeLongitude",
                    Value = destinationCoordinates
                },
            },
            Mode = LoadBalancingMode.RoundRobin
        });

        var res = await channel!.ReadAsync();

        IEnumerable<StmBusDto?>? busDto = JsonConvert.DeserializeObject<IEnumerable<StmBusDto>>(res.Content);

        return busDto;
    }

    public async Task BeginTracking(IStmBus? stmBus)
    {
        if(stmBus is not StmBusDto busDto) throw new InvalidDataContractException("Make sure to not alter the type stored in the collection returned by GetBestBus");

        _ = await RestController.Post(new PostRoutingRequest<StmBusDto>()
        {
            TargetService = "STM",
            Endpoint = $"Track/BeginTracking",
            Payload = busDto,
            Mode = LoadBalancingMode.RoundRobin
        });
    }

    public async Task<IBusTracking?> GetTrackingUpdate(string busId)
    {
        try
        {
            var res = await RestController.Get(new GetRoutingRequest()
            {
                TargetService = "STM",
                Endpoint = $"Track/GetTrackingUpdate",
                Params = new List<NameValue>()
                {
                    new ()
                    {
                        Name = "busId",
                        Value = busId
                    }
                },
                Mode = LoadBalancingMode.RoundRobin
            });

            var data = await res!.ReadAsync();

            if (data.IsSuccessStatusCode && data.StatusCode.Equals(HttpStatusCode.NoContent) is false)
            {
                var busTracking = JsonConvert.DeserializeObject<BusTracking>(data.Content);

                return busTracking;
            }

            _logger?.LogInformation($"{nameof(GetTrackingUpdate)} resulted in {data.StatusDescription}");
        }
        catch (Exception e)
        {
            _logger?.LogError(e.ToString());
        }

        return null;
    }
}