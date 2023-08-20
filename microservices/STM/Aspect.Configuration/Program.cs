using Application.Commands.Seedwork;
using Application.CommandServices.HostedServices.Processors;
using Application.Queries.Seedwork;
using Aspect.Configuration.Dispatchers;
using Controllers.Rest;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Application.CommandServices.ServiceInterfaces;
using Application.CommandServices.ServiceInterfaces.Repositories;
using Application.EventHandlers.AntiCorruption;
using Domain.Events.Interfaces;
using Domain.Events;
using Infrastructure.ApiClients;
using Infrastructure.FileHandlers.Gtfs;
using Infrastructure.ReadRepositories;
using Infrastructure.TcpClients;
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
using Application.CommandServices.HostedServices.Workers;
using Application.CommandServices.Seedwork;
using Application.QueryServices;
using Application.QueryServices.Seedwork;

namespace Aspect.Configuration
{
    public class Program
    {
        public static Action<DbContextOptionsBuilder, IConfiguration> RepositoryDbContextOptionConfiguration { get; set; }

        public static Action<IServiceCollection, IConfiguration> Configuration { get; set; } = ConfigurationSetup;
        public static Action<IServiceCollection> Presentation { get; set; } = PresentationSetup;
        public static Action<IServiceCollection, IConfiguration> Infrastructure { get; set; } = InfrastructureSetup;
        public static Action<IServiceCollection> Application { get; set; } = ApplicationSetup;
        public static Action<IServiceCollection> Domain { get; set; } = DomainSetup;

        public static void Main(string[] args)
        {
            Environment.SetEnvironmentVariable("API_KEY", "l7f41468f7c35f4bd39523510d89637523");

            var builder = WebApplication.CreateBuilder(args);

            RepositoryDbContextOptionConfiguration = (options, builderConfiguration) =>
            {
                options.UseInMemoryDatabase("InMemory");
                options.EnableSensitiveDataLogging();
            };

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
            app.Run();
        }

        public static void ConfigureServices(IServiceCollection services, IConfiguration builderConfiguration)
        {
            Configuration(services, builderConfiguration);
            Infrastructure(services, builderConfiguration);
            Presentation(services);
            Application(services);
            Domain(services);

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

            services.AddScoped<ICommandDispatcher, CommandDispatcher>();
            services.AddScoped<IQueryDispatcher, QueryDispatcher>();

            services.AddHostedService<BusUpdateService>();
            services.AddHostedService<TripUpdateService>();
            services.AddHostedService<LoadStaticGtfsService>();
            services.AddHostedService<RideTrackingService>();

            services.AddScoped<LoadStaticGtfsProcessor>();
            services.AddScoped<RideTrackingProcessor>();
            services.AddScoped<BusUpdateProcessor>();
            services.AddScoped<TripUpdateProcessor>();

            services.AddScoped<ApplicationStopService>();
            services.AddScoped<ApplicationBusServices>();
            services.AddScoped<ApplicationTripServices>();
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

            ScrutorScanForType(services, typeof(IDomainEventHandler<>), assemblyNames: "Application.EventHandlers");
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