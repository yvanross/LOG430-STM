using System.Threading.Channels;
using ApplicationLogic.Usecases;
using Entities.DomainInterfaces;
using MassTransit;
using MqContracts;
using Polly;

namespace TripComparator.Controllers;

public class TripComparatorMqController : IConsumer<CoordinateMessage>
{
    private readonly CompareTimes _compareTimes;

    private readonly ILogger<TripComparatorMqController> _logger;

    public TripComparatorMqController(ILogger<TripComparatorMqController> logger, CompareTimes compareTimes)
    {
        _logger = logger;
        _compareTimes = compareTimes;
    }

    public async Task Consume(ConsumeContext<CoordinateMessage> context)
    {
        //Todo tactique disponibilité retry
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryForeverAsync(retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(retryAttempt, 2)),
                (exception, delay, retryCount)
                    => _logger.LogCritical($"{nameof(Consume)} of {nameof(TripComparatorMqController)} failed with exception: {exception.Message}." +
                                           $" Waiting {delay} ms before next retry. Retry attempt {retryCount}."));

        string startingCoordinates = context.Message.StartingCoordinates, destinationCoordinates = context.Message.DestinationCoordinates;

        _logger.LogInformation($"Comparing trip duration from {startingCoordinates} to {destinationCoordinates}");

        var producer = await retryPolicy.ExecuteAsync(async () => await _compareTimes.BeginComparingBusAndCarTime(
            RemoveWhiteSpaces(startingCoordinates),
            RemoveWhiteSpaces(destinationCoordinates)));

        _ = retryPolicy.ExecuteAsync(async () => await _compareTimes.PollTrackingUpdate(producer.Writer));

        _ = _compareTimes.WriteToStream(producer.Reader);

        string RemoveWhiteSpaces(string s) => s.Replace(" ", "");
    }
}