using System.Reflection;
using System.Resources;
using Application.Commands.LoadStaticGtfs;
using Application.Commands.Seedwork;
using Application.Commands.UpdateBus;
using Application.Commands.UpdateRidesTracking;
using Application.Commands.UpdateTrips;
using Application.CommandServices;
using Application.CommandServices.Interfaces;
using Application.CommandServices.Repositories;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Policies;
using Application.EventHandlers.Interfaces;
using Application.EventHandlers.Messaging;
using Application.Mapping.Interfaces;
using Application.Queries.Seedwork;
using Application.QueryServices;
using Application.QueryServices.ServiceInterfaces;
using Configuration.Dispatchers;
using Configuration.Policies;
using Configuration.Properties;
using Controllers.Jobs;
using Controllers.Rest;
using Domain.Common;
using Domain.Common.Interfaces;
using Domain.Events;
using Domain.Events.Interfaces;
using Domain.Services.Aggregates;
using Domain.Services.Utility;
using Infrastructure.ApiClients;
using Infrastructure.Events;
using Infrastructure.FileHandlers.Gtfs;
using Infrastructure.FileHandlers.Gtfs.Wrappers;
using Infrastructure.ReadRepositories;
using Infrastructure.WriteRepositories;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.OpenApi.Models;

namespace Configuration;

public class Program
{
    public static bool UseInMemoryDatabase = false;

    private static readonly InMemoryDatabaseRoot _databaseRoot = new();

    public static Action<DbContextOptionsBuilder> RepositoryDbContextOptionConfiguration { get; set; }

    public static Action<IServiceCollection, IConfiguration> Configuration { get; set; } = ConfigurationSetup;
    public static Action<IServiceCollection> Presentation { get; set; } = PresentationSetup;
    public static Action<IServiceCollection, IConfiguration> Infrastructure { get; set; } = InfrastructureSetup;
    public static Action<IServiceCollection> Application { get; set; } = ApplicationSetup;
    public static Action<IServiceCollection> Domain { get; set; } = DomainSetup;

    public static void Main(string[] args)
    {
        Environment.SetEnvironmentVariable("API_KEY", "l7f41468f7c35f4bd39523510d89637523");

        var builder = WebApplication.CreateBuilder(args);

        RepositoryDbContextOptionConfiguration = UseInMemoryDatabase
            ? (options) => { options.UseInMemoryDatabase("InMemory", _databaseRoot); }
            : (options) =>
            {
                options.UseNpgsql("Server=host.docker.internal;Port=32672;Username=postgres;Password=secret;Database=STM;");
            };

        builder.Services.AddLogging(builder =>
        {
            // no need for every ef core commands to pass through the logger
            builder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
        });

        // Add services to the container.
        ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();

        app.UseSwagger();

        app.UseSwaggerUI();

        app.UseCors(
            options =>
            {
                options.AllowAnyOrigin();
                options.AllowAnyHeader();
                options.AllowAnyMethod();
            }
        );

        app.MapControllers();

        var scope = app.Services.CreateScope();

        if (scope.ServiceProvider.GetRequiredService<AppReadDbContext>().Database.IsInMemory() is false)
            scope.ServiceProvider.GetRequiredService<AppReadDbContext>().Database.Migrate();

        if (scope.ServiceProvider.GetRequiredService<EventDbContext>().Database.IsInMemory() is false)
            scope.ServiceProvider.GetRequiredService<EventDbContext>().Database.Migrate();

        app.Run();
    }

    public static void ConfigureServices(IServiceCollection services, IConfiguration builderConfiguration)
    {
        Domain(services);
        Application(services);
        Infrastructure(services, builderConfiguration);
        Presentation(services);
        Configuration(services, builderConfiguration);

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "STM", Version = "v1" });
        });
    }

    private static void ConfigurationSetup(IServiceCollection services, IConfiguration builderConfiguration)
    {
        services.AddScoped(typeof(IInfiniteRetryPolicy<>), typeof(InfiniteRetryPolicy<>));
        services.AddScoped(typeof(IBackOffRetryPolicy<>), typeof(BackOffRetryPolicy<>));

        services.AddSingleton(_ => new ResourceManager(typeof(Resources)));

        services.AddSingleton<IHostInfo, HostInfo>();

        services.AddSingleton<IDataReader, DataReader>();
    }

    private static void PresentationSetup(IServiceCollection services)
    {
        services.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(typeof(FinderController).Assembly));

        services.AddHostedService<BusUpdateJob>();
        services.AddHostedService<UpdateTripsJob>();
        services.AddHostedService<LoadStaticGtfsJob>();
        services.AddHostedService<RideTrackingJob>();
        services.AddHostedService<InitializationHealthJob>();
    }

    private static void InfrastructureSetup(IServiceCollection services, IConfiguration configuration)
    {
        if (RepositoryDbContextOptionConfiguration is null)
            throw new NullReferenceException("RepositoryDbContextOptionConfiguration is null");

        void RepositoryGenericConfiguration(DbContextOptionsBuilder options)
        {
            RepositoryDbContextOptionConfiguration(options);
        }

        services.AddDbContext<AppReadDbContext>(RepositoryGenericConfiguration);
        services.AddDbContext<AppWriteDbContext>(RepositoryGenericConfiguration);
        services.AddDbContext<EventDbContext>(RepositoryGenericConfiguration);

        ScrutorScanForType(services, typeof(IWriteRepository<>), assemblyNames: "Infrastructure.WriteRepositories");

        services.AddScoped<IEventContext, EventDbContext>();

        services.AddScoped<IQueryContext, AppReadDbContext>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IStmClient, StmClient>();

        services.AddScoped<ITransitDataReader, TransitDataReader>();
        services.AddScoped<GtfsFileFileCache>();
        services.AddScoped<WrapperMediator>();

        services.AddSingleton<IConsumer, InMemoryEventQueue>();
        services.AddSingleton<IPublisher, InMemoryEventQueue>();
    }

    private static void ApplicationSetup(IServiceCollection services)
    {
        ScrutorScanForType(services, typeof(IQueryHandler<,>), assemblyNames: "Application.Queries");
        ScrutorScanForType(services, typeof(ICommandHandler<>), assemblyNames: "Application.Commands");
        ScrutorScanForType(services, typeof(ICommand), assemblyNames: "Application.Commands");
        ScrutorScanForType(services, typeof(IQuery), assemblyNames: "Application.Queries");

        ScrutorScanForType(services, typeof(IMappingTo<,>), assemblyNames: "Application.Mapping");

        ScrutorScanForType(services, typeof(IApplicationEventHandler<>), ServiceLifetime.Scoped, "Application.EventHandlers");

        services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
        services.AddSingleton<IQueryDispatcher, QueryDispatcher>();

        services.AddScoped<LoadStaticGtfsHandler>();
        services.AddScoped<UpdateRidesTrackingHandler>();
        services.AddScoped<UpdateBusesHandler>();
        services.AddScoped<UpdateTripsHandler>();

        services.AddScoped<ApplicationStopService>();
        services.AddScoped<ApplicationBusServices>();
        services.AddScoped<ApplicationTripService>();

        //_ = UseInMemoryDatabase
        //    ? services.AddScoped<IApplicationTripService, ApplicationTripServiceInMemory>()
        //    : services.AddScoped<IApplicationTripService, ApplicationTripService>();
    }

    private static void DomainSetup(IServiceCollection services)
    {
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddSingleton<IDatetimeProvider, DateTimeProvider>();

        services.AddSingleton<BusServices>();
        services.AddSingleton<RideServices>();
        services.AddSingleton<StopServices>();
        services.AddSingleton<TripServices>();

        services.AddSingleton<TimeServices>();

        ScrutorScanForType(services, typeof(IDomainEventHandler<>), ServiceLifetime.Scoped,
            "Application.EventHandlers");
    }

    private static void ScrutorScanForType(IServiceCollection services, Type type,
        ServiceLifetime lifetime = ServiceLifetime.Scoped, params string[] assemblyNames)
    {
        services.Scan(selector =>
        {
            selector.FromAssemblies(assemblyNames.Select(Assembly.Load))
                .AddClasses(filter => filter.AssignableTo(type))
                .AsImplementedInterfaces()
                .WithLifetime(lifetime);
        });
    }
}