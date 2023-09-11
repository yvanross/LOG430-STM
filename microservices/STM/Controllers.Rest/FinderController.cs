using Application.Queries.GetEarliestBus;
using Application.Queries.Seedwork;
using Application.ViewModels;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Controllers.Rest;

[ApiController]
[Route("[controller]/[action]")]
public class FinderController : ControllerBase
{
    private readonly ILogger<FinderController> _logger;
    private readonly IQueryDispatcher _queryDispatcher;

    public FinderController(ILogger<FinderController> logger, IQueryDispatcher queryDispatcher)
    {
        _logger = logger;
        _queryDispatcher = queryDispatcher;
    }

    /// <remarks>
    ///     Find optimal STM bus to travel between two coordinates
    /// </remarks>
    /// <param name="fromLatitudeLongitude">Latitude, longitude of the first stop</param>
    /// <param name="toLatitudeLongitude">Latitude, longitude of the last stop</param>
    /// <returns>
    ///     <see cref="RideViewModel" /> object containing information concerning the optimal bus to take
    /// </returns>
    [HttpGet]
    [ActionName(nameof(OptimalBuses))]
    public async Task<ActionResult<RideViewModel>> OptimalBuses(string fromLatitudeLongitude, string toLatitudeLongitude)
    {
        _logger.LogInformation($"OptimalBus endpoint called with coordinated: from: {fromLatitudeLongitude}; to: {toLatitudeLongitude}");

        var (fromLatitude, fromLongitude, toLatitude, toLongitude) = ParseParams();

        var from = new Position(fromLatitude, fromLongitude);
        var to = new Position(toLatitude, toLongitude);

        var ride = await _queryDispatcher.Dispatch<GetEarliestBusQuery, RideViewModel>(new GetEarliestBusQuery(from, to), CancellationToken.None);

        return Ok(ride);

        (double fromLatitude, double fromLongitude, double toLatitude, double toLongitude) ParseParams()
        {
            var fromPositionStrings = fromLatitudeLongitude.Split(',');
            var toPositionStrings = toLatitudeLongitude.Split(',');

            double.TryParse(fromPositionStrings[0].Trim(), out var formattedFromLat);
            double.TryParse(fromPositionStrings[1].Trim(), out var formattedFromLon);
            double.TryParse(toPositionStrings[0].Trim(), out var formattedToLat);
            double.TryParse(toPositionStrings[1].Trim(), out var formattedToLon);

            return (formattedFromLat, formattedFromLon, formattedToLat, formattedToLon);
        }
    }
}