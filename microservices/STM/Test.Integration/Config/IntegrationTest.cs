using Application.Commands.Seedwork;
using Application.EventHandlers.Interfaces;
using Application.Queries.Seedwork;
using Infrastructure.Events;
using Infrastructure.ReadRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Tests.Integration.Config;

public abstract class IntegrationTest
{
    protected readonly ICommandDispatcher CommandDispatcher;
    protected readonly IConsumer Consumer;
    protected readonly ITestOutputHelper OutputHelper;
    protected readonly IQueryDispatcher QueryDispatcher;

    protected IntegrationTest(ITestOutputHelper outputHelper)
    {
        OutputHelper = outputHelper;

        var factory = new IntegrationWebApplicationFactory(outputHelper);

        factory.InitializeAsync().Wait();

        var scope = factory.Services.CreateScope();

        QueryDispatcher = scope.ServiceProvider.GetRequiredService<IQueryDispatcher>();
        CommandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();
        Consumer = scope.ServiceProvider.GetRequiredService<IConsumer>();
        var readDbContext = scope.ServiceProvider.GetRequiredService<AppReadDbContext>();
        var eventDbContext = scope.ServiceProvider.GetRequiredService<EventDbContext>();

        readDbContext.Database.Migrate();
        eventDbContext.Database.Migrate();
    }
}