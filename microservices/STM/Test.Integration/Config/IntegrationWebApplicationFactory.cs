using Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;
using Xunit.Abstractions;

namespace Tests.Integration.Config;

public class IntegrationWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithPortBinding(32573,5432)
        .WithDatabase("stm")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private readonly ITestOutputHelper _outputHelper;

    private const string ApiKey = "l7f41468f7c35f4bd39523510d89637523";

    public IntegrationWebApplicationFactory(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    public Task InitializeAsync()
    {
        return _container.StartAsync();
    }

    public new Task DisposeAsync()
    {
        return _container.StopAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("API_KEY", ApiKey);
        Environment.SetEnvironmentVariable("BASE_PATH", @"..\..\..\..\Configuration\Data\");
        Environment.SetEnvironmentVariable("MEMORY_CONSUMPTION", "HIGH");

        builder
            .ConfigureAppConfiguration((_, config) =>
            {
                var appSettingsPath = "/app/appsettings.json";

                config.AddJsonFile(appSettingsPath, true, true);
            })
            .ConfigureServices((hostingContext, services) =>
            {
                Program.RepositoryDbContextOptionConfiguration = (options) =>
                {
                    options.UseNpgsql(_container.GetConnectionString());
                };

                Program.ConfigureServices(services, hostingContext.Configuration);

                services.AddSingleton(_outputHelper);
            })
            .ConfigureLogging(loggingBuilder =>
            {
                loggingBuilder.Services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            });
    }
}

public class Logger<T> : ILogger<T>
{
    private readonly ITestOutputHelper _outputHelper;

    public Logger(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        _outputHelper.WriteLine(formatter(state, exception));
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }
}