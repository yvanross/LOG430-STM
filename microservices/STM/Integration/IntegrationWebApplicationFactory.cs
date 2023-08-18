using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Integration;

public class IntegrationWebApplicationFactory : WebApplicationFactory<Aspect.Configuration.Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("stm")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(_container.ConnectionString);
            });
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