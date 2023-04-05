using ApplicationLogic.Usecases;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using MqContracts;
using System.Threading.Channels;
using Polly;
using TripComparator.DTO;
using TripComparator.External;

namespace TripComparator.Controllers;

public class TripComparatorMqController : IConsumer<CoordinateMessage>
{
    private readonly IPublishEndpoint _publishEndpoint;

    private readonly ILogger<TripComparatorMqController> _logger;

    public TripComparatorMqController(ILogger<TripComparatorMqController> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<CoordinateMessage> context)
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryForeverAsync(retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(retryAttempt, 2)),
                (exception, delay, retryCount)
                    => _logger.LogCritical($"{nameof(Consume)} of {nameof(TripComparatorMqController)} failed with exception: {exception.Message}." +
                                           $" Waiting {delay} ms before next retry. Retry attempt {retryCount}."));

        var compareTimesUc = new CompareTimesUC(
            new RouteTimeProviderClient(),
            new StmClient(_logger),
            new MassTransitRabbitMqClient(_publishEndpoint),
            _logger);

        string startingCoordinates = context.Message.StartingCoordinates, destinationCoordinates = context.Message.DestinationCoordinates;

        _logger.LogInformation($"Comparing trip duration from {startingCoordinates} to {destinationCoordinates}");

        var producer = await compareTimesUc.BeginComparingBusAndCarTime(
            RemoveWhiteSpaces(startingCoordinates), 
            RemoveWhiteSpaces(destinationCoordinates));

        _ = retryPolicy.ExecuteAsync(async () => await compareTimesUc.PollTrackingUpdate(producer.Writer));

        _ = compareTimesUc.WriteToStream(producer.Reader);

        string RemoveWhiteSpaces(string s) => s.Replace(" ", "");
    }
}