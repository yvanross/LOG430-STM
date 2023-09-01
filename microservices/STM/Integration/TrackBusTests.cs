using Application.Commands;
using Application.Queries;
using Application.ViewModels;
using Contracts;
using Domain.ValueObjects;
using Integration.Config;
using Xunit.Abstractions;

namespace Integration;

public class TrackBusTests : IntegrationTest
{
    public TrackBusTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
        Environment.SetEnvironmentVariable("API_KEY", "l7f41468f7c35f4bd39523510d89637523");
    }

    [Fact]
    public async Task Should_Begin_Tracking_Bus()
    {
        var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(25000));

        var consumeTasks = new Task[] {
            Consumer.ConsumeNext<StaticGtfsDataLoaded>(cancellationToken.Token),
            Consumer.ConsumeNext<BusPositionsUpdated>(cancellationToken.Token),
            Consumer.ConsumeNext<StmTripModificationApplied>(cancellationToken.Token)
        };

        await Task.WhenAll(consumeTasks);

        OutputHelper.WriteLine("All events consumed");

        var initialPosition = new Position(45.49536173070412, -73.563059502814810);

        var destination = new Position(45.50174862045254, -73.57655617412391);

        var query = new GetEarliestBus(initialPosition, destination);

        var rideInfo = await QueryDispatcher.Dispatch<GetEarliestBus, RideViewModel>(query, CancellationToken.None);

        var command = new TrackBus(rideInfo.BusId, rideInfo.ScheduledDepartureId, rideInfo.ScheduledDestinationId);

        await CommandDispatcher.DispatchAsync(command, CancellationToken.None);

        var cancellationTokenRide = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var update = await Consumer.ConsumeNext<RideTrackingUpdated>(cancellationTokenRide.Token);

        Assert.NotNull(update);

        Assert.False(update.TrackingCompleted);

        Assert.NotEmpty(update.Message);
    }
}