using Application.Commands;
using Application.CommandServices.Repositories;
using Application.Queries;
using Application.QueryServices.ServiceInterfaces;
using Application.ViewModels;
using Contracts;
using Domain.ValueObjects;
using Google.Protobuf.WellKnownTypes;
using Infrastructure.Events;
using Infrastructure.ReadRepositories;
using Integration.Config;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Integration;

public class TrackBusTests : IntegrationTest
{
    public TrackBusTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
        Environment.SetEnvironmentVariable("API_KEY", "l7f41468f7c35f4bd39523510d89637523");
    }

    //this is a very long running test and since it depends on real time data, it might return false negatives, read the log to see where it failed
    // it is more of a proof of concept than a real test
    [Fact]
    public async Task Should_Begin_Tracking_Bus()
    {
        var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(25000));

        await Consumer.ConsumeNext<ServiceInitialized>(cancellationToken.Token);

        var initialPosition = new Position(45.49536173070412, -73.563059502814810);

        var destination = new Position(45.50174862045254, -73.57655617412391);

        var query = new GetEarliestBus(initialPosition, destination);

        OutputHelper.WriteLine("Querying for earliest bus");

       cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(120));

        RideViewModel rideInfo;

        while (cancellationToken.IsCancellationRequested is false)
        {
            try
            {
                rideInfo = await QueryDispatcher.Dispatch<GetEarliestBus, RideViewModel>(query, CancellationToken.None);
            }
            catch (Exception e)
            {
                OutputHelper.WriteLine(e.Message);
            }

            break;
        }

        OutputHelper.WriteLine($"Found bus {rideInfo.BusId} at {rideInfo.ScheduledDepartureId} going to {rideInfo.ScheduledDestinationId}");

        var command = new TrackBus(rideInfo.BusId, rideInfo.ScheduledDepartureId, rideInfo.ScheduledDestinationId);

        await CommandDispatcher.DispatchAsync(command, CancellationToken.None);

        OutputHelper.WriteLine("Tracking bus");

        var cancellationTokenRide = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var update = await Consumer.ConsumeNext<RideTrackingUpdated>(cancellationTokenRide.Token);

        OutputHelper.WriteLine(update.Message);

        Assert.NotNull(update);

        Assert.False(update.TrackingCompleted);

        Assert.NotEmpty(update.Message);
    }
}