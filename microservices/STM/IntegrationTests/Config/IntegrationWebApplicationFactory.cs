using Aspect.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;

namespace IntegrationTests.Config;

public class IntegrationWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("stm")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

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
        });
    }

    [TestInitialize]
    public async Task Initialize()
    {
        await _container.StartAsync();
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        await _container.StopAsync();
    }
}