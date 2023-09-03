using Application.Commands.Seedwork;
using Application.Queries.Seedwork;
using Aspect.Configuration.Dispatchers;
using Controllers.Rest;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Application.EventHandlers.AntiCorruption;
using Domain.Events.Interfaces;
using Domain.Events;
using Infrastructure.ApiClients;
using Infrastructure.FileHandlers.Gtfs;
using Infrastructure.ReadRepositories;
using Infrastructure.WriteRepositories;
using Application.Common.Interfaces.Policies;
using Application.Mapping.Interfaces;
using Aspect.Configuration.Policies;
using Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Infrastructure.FileHandlers.Gtfs.Wrappers;
using Domain.Common;
using Domain.Services.Aggregates;
using Domain.Services.Utility;
using System.Reflection;
using Aspect.Configuration.Properties;
using System.Resources;
using Application.Commands.Handlers;
using Application.QueryServices;
using Application.QueryServices.ServiceInterfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Application.EventHandlers.Messaging;
using Application.CommandServices.Repositories;
using Application.CommandServices;
using Controllers.Jobs;

namespace Aspect.Configuration
{
    public class Program
    {
        public static bool UseInMemoryDatabase = false;

        public static Action<DbContextOptionsBuilder, IConfiguration> RepositoryDbContextOptionConfiguration { get; set; }

        public static Action<IServiceCollection, IConfiguration> Configuration { get; set; } = ConfigurationSetup;
        public static Action<IServiceCollection> Presentation { get; set; } = PresentationSetup;
        public static Action<IServiceCollection, IConfiguration> Infrastructure { get; set; } = InfrastructureSetup;
        public static Action<IServiceCollection> Application { get; set; } = ApplicationSetup;
        public static Action<IServiceCollection> Domain { get; set; } = DomainSetup;

        private static InMemoryDatabaseRoot _databaseRoot = new ();

        public static void Main(string[] args)
        {
            Environment.SetEnvironmentVariable("API_KEY", "l7f41468f7c35f4bd39523510d89637523");

            var builder = WebApplication.CreateBuilder(args);


            RepositoryDbContextOptionConfiguration = UseInMemoryDatabase? 
                (options, builderConfiguration) =>
                {
                    options.UseInMemoryDatabase("InMemory", _databaseRoot);
                } :
                (options, builderConfiguration) =>
                {
                    options.UseNpgsql("Server=host.docker.internal;Port=32672;Username=postgres;Password=secret;Database=postgres2;");

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

            app.UseAuthorization();
            app.MapControllers();

            if(app.Services.CreateScope().ServiceProvider.GetRequiredService<AppReadDbContext>().Database.IsInMemory() is false) 
                app.Services.CreateScope().ServiceProvider.GetRequiredService<AppReadDbContext>().Database.Migrate();

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

            services.AddSwaggerGen();
        }

        private static void ConfigurationSetup(IServiceCollection services, IConfiguration builderConfiguration)
        {
            services.AddScoped(typeof(IInfiniteRetryPolicy<>), typeof(InfiniteRetryPolicy<>));
            services.AddScoped(typeof(IBackOffRetryPolicy<>), typeof(BackOffRetryPolicy<>));

            services.AddSingleton(_ => new ResourceManager(typeof(Resources)));

            services.AddSingleton<IDataReader, DataReader>();
        }

        private static void PresentationSetup(IServiceCollection services)
        {
            services.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(typeof(FinderController).Assembly));

            services.AddHostedService<BusUpdateJob>();
            services.AddHostedService<UpdateTripsJob>();
            services.AddHostedService<LoadStaticGtfsJob>();
            services.AddHostedService<RideTrackingJob>();
        }

        private static void InfrastructureSetup(IServiceCollection services, IConfiguration configuration)
        {
            if(RepositoryDbContextOptionConfiguration is null)
                throw new NullReferenceException("RepositoryDbContextOptionConfiguration is null");

            void RepositoryGenericConfiguration(DbContextOptionsBuilder options) => RepositoryDbContextOptionConfiguration(options, configuration);

            services.AddDbContext<AppReadDbContext>(RepositoryGenericConfiguration);
            services.AddDbContext<AppWriteDbContext>(RepositoryGenericConfiguration);

            ScrutorScanForType(services, typeof(IWriteRepository<>), assemblyNames: "Infrastructure.WriteRepositories");
            ScrutorScanForType(services, typeof(IReadRepository<>), assemblyNames: "Infrastructure.ReadRepositories");

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
            ScrutorScanForType(services, typeof(IQuery<>), assemblyNames: "Application.Queries");

            ScrutorScanForType(services, typeof(IMappingTo<,>), assemblyNames: "Application.Mapping");

            services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
            services.AddSingleton<IQueryDispatcher, QueryDispatcher>();

            services.AddScoped<LoadStaticGtfsHandler>();
            services.AddScoped<UpdateRideTrackingHandler>();
            services.AddScoped<UpdateBusesHandler>();
            services.AddScoped<UpdateTripsHandler>();

            services.AddScoped<ApplicationStopService>();
            services.AddScoped<ApplicationBusServices>();

            _ = UseInMemoryDatabase ?
                services.AddScoped<IApplicationTripService, ApplicationTripServiceInMemory>() :
                services.AddScoped<IApplicationTripService, ApplicationTripService>();
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

            ScrutorScanForType(services, typeof(IDomainEventHandler<>), ServiceLifetime.Scoped, "Application.EventHandlers");
        }

        private static void ScrutorScanForType(IServiceCollection services, Type type, ServiceLifetime lifetime = ServiceLifetime.Scoped, params string[] assemblyNames)
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
}