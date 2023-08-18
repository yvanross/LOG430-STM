using Application.Commands.Seedwork;
using Application.CommandServices.HostedServices;
using Application.Queries.Seedwork;
using Aspect.Configuration.Dispatchers;
using Controllers.Rest;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Application.CommandServices.ServiceInterfaces;
using Application.CommandServices.ServiceInterfaces.Repositories;
using Application.EventHandlers.AntiCorruption;
using Application.QueryServices.ServiceInterfaces.Repositories;
using Domain.Events.Interfaces;
using Domain.Events;
using Infrastructure.ApiClients;
using Infrastructure.FileHandlers.Gtfs;
using Infrastructure.ReadRepositories;
using Infrastructure.TcpClients;
using Infrastructure.WriteRepositories;
using Application.Common.Interfaces.Policies;
using Aspect.Configuration.Policies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Aspect.Configuration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

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

        public static void ConfigureServices(IServiceCollection services, ConfigurationManager builderConfiguration)
        {
            Configuration(services);
            Infrastructure(builderConfiguration, services);
            Presentation(services);
            Application(services);
            Domain(services);

            services.AddControllers();

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen();
        }

        private static void Presentation(IServiceCollection services)
        {
            services.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(typeof(FinderController).Assembly));
        }

        private static void Configuration(IServiceCollection services)
        {
            services.AddScoped(typeof(IInfiniteRetryPolicy<>), typeof(InfiniteRetryPolicy<>));
            services.AddScoped(typeof(IBackOffRetryPolicy<>), typeof(BackOffRetryPolicy<>));
        }

        private static void Infrastructure(ConfigurationManager builderConfiguration, IServiceCollection services)
        {
            var dataReader = new DataReader();

            builderConfiguration.Bind("DataFilePath", dataReader);

            services.AddSingleton<IDataReader>(dataReader);

            void RepositoryGenericConfiguration(DbContextOptionsBuilder options) => RepositoryDbContextOptionConfiguration(options, builderConfiguration);

            services.AddDbContext<BusWriteRepository>(RepositoryGenericConfiguration);
            services.AddDbContext<RideWriteRepository>(RepositoryGenericConfiguration);
            services.AddDbContext<StopWriteRepository>(RepositoryGenericConfiguration);
            services.AddDbContext<TripWriteRepository>(RepositoryGenericConfiguration);

            services.AddDbContext<BusReadRepository>(RepositoryGenericConfiguration);
            services.AddDbContext<RideReadRepository>(RepositoryGenericConfiguration);
            services.AddDbContext<StopReadRepository>(RepositoryGenericConfiguration);
            services.AddDbContext<TripReadRepository>(RepositoryGenericConfiguration);

            services.AddScoped<IBusWriteRepository, BusWriteRepository>();
            services.AddScoped<IRideWriteRepository, RideWriteRepository>();
            services.AddScoped<IStopWriteRepository, StopWriteRepository>();
            services.AddScoped<ITripWriteRepository, TripWriteRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IBusReadRepository, BusReadRepository>();
            services.AddScoped<IRideReadRepository, RideReadRepository>();
            services.AddScoped<IStopReadRepository, StopReadRepository>();
            services.AddScoped<ITripReadRepository, TripReadRepository>();

            services.AddScoped<IStmClient, StmClient>();
            services.AddScoped<ITransitDataReader, TransitDataReader>();

            services.AddScoped<IConsumer, InMemoryEventQueue>();
            services.AddScoped<IPublisher, InMemoryEventQueue>();
        }

        private static void Application(IServiceCollection services)
        {
            ScrutorScanForType(services, typeof(IQueryHandler<,>));
            ScrutorScanForType(services, typeof(ICommandHandler<>));
            ScrutorScanForType(services, typeof(ICommand));
            ScrutorScanForType(services, typeof(IQuery<>));

            services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
            services.AddSingleton<IQueryDispatcher, QueryDispatcher>();

            services.AddHostedService<BusUpdateService>();
            services.AddHostedService<TripUpdateService>();
            services.AddHostedService<LoadStaticGtfsHostedService>();
            services.AddHostedService<RideTrackingService>();
        }

        private static void Domain(IServiceCollection services)
        {
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

            ScrutorScanForType(services, typeof(IDomainEventHandler<>));
        }

        private static void ScrutorScanForType(IServiceCollection services, Type type)
        {
            services.Scan(selector =>
            {
                selector.FromCallingAssembly()
                    .AddClasses(filter => { filter.AssignableTo(type); })
                    .AsImplementedInterfaces()
                    .WithScopedLifetime();
            });
        }

        private static void RepositoryDbContextOptionConfiguration(DbContextOptionsBuilder options, ConfigurationManager builderConfiguration)
        {
            options.UseInMemoryDatabase(builderConfiguration.GetConnectionString("InMemory")!);
        }
    }
}