using Application.Commands;
using Application.Commands.Seedwork;
using Application.EventHandlers.AntiCorruption;
using Application.Queries;
using Application.Queries.Seedwork;
using Application.ViewModels;
using Aspect.Configuration;
using Contracts;
using Docker.DotNet.Models;
using Domain.ValueObjects;
using IntegrationTests.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests;

[TestClass]
public class TrackBusTests
{
    private readonly TestServer _server;
    private readonly PostgreSqlContainerSetup _containerSetup;

    private readonly IQueryDispatcher QueryDispatcher;
    private readonly ICommandDispatcher CommandDispatcher;
    private readonly IConsumer Consumer;

    private readonly IServiceScope Scope;

    public TrackBusTests()
    {
        var builder = WebApplication.CreateBuilder();

        // Add custom configuration, if needed.
        // ...

        // Configure services, similar to your Program class.
        Program.ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();

        // Configure your app, similar to your Program class.
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors(options =>
        {
            options.AllowAnyOrigin();
            options.AllowAnyHeader();
            options.AllowAnyMethod();
        });
        app.UseAuthorization();
        app.MapControllers();

        _containerSetup = new PostgreSqlContainerSetup();

        Scope = app.Services.CreateScope();

        QueryDispatcher = Scope.ServiceProvider.GetRequiredService<IQueryDispatcher>();
        CommandDispatcher = Scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();
        Consumer = Scope.ServiceProvider.GetRequiredService<IConsumer>();
    }

    [TestInitialize]
    public async Task Initialize()
    {
        await _containerSetup.InitializeAsync();
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        await _containerSetup.DisposeAsync();
    }

    [TestMethod]
    public async Task Should_Begin_Tracking_Bus()
    {
        await Task.Delay(TimeSpan.FromSeconds(60));

        var consumeTasks = new Task[] {
            Consumer.ConsumeNext<StaticGtfsDataLoaded>(),
            Consumer.ConsumeNext<BusPositionsUpdated>(),
            Consumer.ConsumeNext<StmTripModificationApplied>()
        };

        await Task.WhenAll(consumeTasks);

        var initialPosition = new Position(45.49536173070412, -73.563059502814810);

        var destination = new Position(45.50174862045254, -73.57655617412391);

        var query = new GetEarliestBus(initialPosition, destination);

        var rideInfo = await QueryDispatcher.Dispatch<GetEarliestBus, RideViewModel>(query, CancellationToken.None);

        var command = new TrackBus(rideInfo.BusId, rideInfo.ScheduledDepartureId, rideInfo.ScheduledDestinationId);

        await CommandDispatcher.Dispatch(command, CancellationToken.None);

        var update = await Consumer.ConsumeNext<RideTrackingUpdated>();

        Assert.IsNotNull(update);

        Assert.IsFalse(update.TrackingCompleted);

        Assert.AreNotEqual(update.Message, string.Empty);
    }
}