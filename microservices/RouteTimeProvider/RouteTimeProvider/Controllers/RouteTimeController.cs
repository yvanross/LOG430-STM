using ApplicationLogic.Usecases;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Polly;
using RouteTimeProvider.External;

namespace RouteTimeProvider.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("[controller]/[action]")]
    public class RouteTimeController : ControllerBase
    {
        private readonly ILogger<RouteTimeController> _logger;

        private readonly CarTravel _carTravel = new ();

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

            var travelTime = await retryPolicy.ExecuteAsync(() => _carTravel.GetTravelTimeInSeconds(startingCoordinates, destinationCoordinates, new TomTomClient()));

            return Ok(travelTime);
        }
    }
}