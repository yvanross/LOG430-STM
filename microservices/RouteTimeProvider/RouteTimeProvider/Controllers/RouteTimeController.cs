using System.Linq.Expressions;
using Ambassador;
using Ambassador.Usecases;
using ApplicationLogic.Usecases;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PLACEHOLDER.External;
using Polly;

namespace PLACEHOLDER.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]/[action]")]
    public class RouteTimeController : ControllerBase
    {
        private readonly ILogger<RouteTimeController> _logger;

        private readonly TravelUC _travelUc = new ();

        public RouteTimeController(ILogger<RouteTimeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ActionName(nameof(Get))]
        public async Task<ActionResult<int>> Get(string startingCoordinates, string destinationCoordinates)
        {
            _logger.LogInformation($"Fetching car travel time from {startingCoordinates} to {destinationCoordinates}");

            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, delay, retryCount, _) =>
                    {
                        _logger.LogError($"Operation failed with exception: {exception.Message}. Waiting {delay} before next retry. Retry attempt {retryCount}.");
                    });

            var travelTime = await retryPolicy.ExecuteAsync(() => _travelUc.GetTravelTimeInSeconds(startingCoordinates, destinationCoordinates, new TomTomClient()));

            return Ok(travelTime);
        }
    }
}