using ApplicationLogic.Usecases;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using MqContracts;
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
        var compareTimesUc = new CompareTimesUC(
            new RouteTimeProviderClient(),
            new StmClient(_logger),
            new MassTransitRabbitMqClient(_publishEndpoint));

        string startingCoordinates = context.Message.StartingCoordinates, destinationCoordinates = context.Message.DestinationCoordinates;

        _logger.LogInformation($"Comparing trip duration from {startingCoordinates} to {destinationCoordinates}");

        var producer = await compareTimesUc.BeginComparingBusAndCarTime(
            RemoveWhiteSpaces(startingCoordinates), 
            RemoveWhiteSpaces(destinationCoordinates));

        _ = compareTimesUc.WriteToStream(producer);

        string RemoveWhiteSpaces(string s) => s.Replace(" ", "");
    }
}