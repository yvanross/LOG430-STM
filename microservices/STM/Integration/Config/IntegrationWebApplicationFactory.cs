using Aspect.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;
using Xunit.Abstractions;

namespace Integration.Config;

public class IntegrationWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly ITestOutputHelper _outputHelper;

    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("stm")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public IntegrationWebApplicationFactory(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
        .ConfigureAppConfiguration((_, config) =>
        {
            var appSettingsPath = "/app/appsettings.json";

            config.AddJsonFile(appSettingsPath, optional: true, reloadOnChange: true);
        })
        .ConfigureServices((hostingContext, services) =>
        {
            Program.RepositoryDbContextOptionConfiguration = (options, _) =>
            {
                options.UseNpgsql(_container.GetConnectionString());
            };

            Program.ConfigureServices(services, hostingContext.Configuration);

            services.AddSingleton<ITestOutputHelper>(_outputHelper);
        })
        .ConfigureLogging(loggingBuilder =>
        {
            loggingBuilder.Services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
        });
    }

    public Task InitializeAsync()
    {
        return _container.StartAsync();
    }

    public new Task DisposeAsync()
    {
        return _container.StopAsync();
    }
}

public class Logger<T> : ILogger<T>
{
    private ITestOutputHelper _outputHelper;

    public Logger(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
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