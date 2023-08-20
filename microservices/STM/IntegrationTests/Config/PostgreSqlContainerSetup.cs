using Testcontainers.PostgreSql;

namespace IntegrationTests.Config;

public class PostgreSqlContainerSetup
{
    public PostgreSqlContainer Container { get; private set; }

    public PostgreSqlContainerSetup()
    {
        Container = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("stm")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await Container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await Container.StopAsync();
    }
}