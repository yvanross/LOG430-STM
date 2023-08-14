using ApplicationLogic.Interfaces;
using Domain.Aggregates;
using Domain.Common.Interfaces;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.Services.Itinerary;

public class ItineraryPlannerService
{
    private ILogger _logger;

    private readonly ITransitDataCache _stmData;
    private readonly IDatetimeProvider _datetimeProvider;

    public ItineraryPlannerService(ILogger<ItineraryPlannerService> logger, ITransitDataCache stmData, IDatetimeProvider datetimeProvider)
    {
        _logger = logger;
        _stmData = stmData;
        _datetimeProvider = datetimeProvider;
    }
}