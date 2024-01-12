using Application.Commands.TrackBus;
using Application.Queries.GetEarliestBus;
using Application.ViewModels;
using Contracts;
using Domain.ValueObjects;
using Tests.Integration.Config;
using Xunit.Abstractions;

namespace Tests.Integration;

public class TrackBusTests : IntegrationTest
{
    public TrackBusTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    //This is a very long running test and since it depends on real time data, it might return false negatives, read the log to see where it failed
    //It is more of a proof of concept than a real test
    //Bear in mind it is long running

    //IMPORTANT ADD API KEY TO ENVIRONMENT VARIABLES IN IntegrationWebApplicationFactory.cs
    [Fact]
    public async Task Should_Begin_Tracking_Bus()
    {
        var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(240));

        await Consumer.ConsumeNext<ServiceInitialized>(cancellationToken.Token);

        var initialPosition = new Position(45.50805689530107, -73.57132679709797);

        var destination = new Position(45.499665217304695, -73.57887989771196);

        var query = new GetEarliestBusQuery(initialPosition, destination);

        RideViewModel rideInfo;

        OutputHelper.WriteLine("Leaving time to cache buses");

        await Task.Delay(TimeSpan.FromSeconds(60));

        OutputHelper.WriteLine("Querying for earliest bus");

        rideInfo = await QueryDispatcher.Dispatch<GetEarliestBusQuery, RideViewModel>(query, CancellationToken.None);

        OutputHelper.WriteLine($"Found bus {rideInfo.BusId} at {rideInfo.ScheduledDepartureId} going to {rideInfo.ScheduledDestinationId}");

        var command = new TrackBusCommand(rideInfo.ScheduledDepartureId, rideInfo.ScheduledDestinationId, rideInfo.BusId);

        await CommandDispatcher.DispatchAsync(command, CancellationToken.None);

        OutputHelper.WriteLine("Tracking bus");

        var cancellationTokenRide = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var update = await Consumer.ConsumeNext<ApplicationRideTrackingUpdated>(cancellationTokenRide.Token);

        OutputHelper.WriteLine(update.Message);

        Assert.NotNull(update);

        Assert.False(update.TrackingCompleted);

        Assert.NotEmpty(update.Message);
    }
}