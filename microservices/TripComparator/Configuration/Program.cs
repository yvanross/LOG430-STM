using Application.Interfaces;
using Application.Interfaces.Policies;
using Application.Usecases;
using Configuration.Policies;
using Controllers.Controllers;
using Infrastructure.Clients;
using MassTransit;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.OpenApi.Models;
using MqContracts;
using RabbitMQ.Client;
using ServiceMeshHelper;
using ServiceMeshHelper.Controllers;

namespace Configuration
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder.Services);

            var app = builder.Build();

            app.UseSwagger();

            app.UseSwaggerUI();

            app.UseHttpsRedirection();

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

            await app.RunAsync();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            ConfigureMassTransit(services);

            services.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(typeof(CompareTripController).Assembly));

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TripComparator", Version = "v1" });
                c.EnableAnnotations();
            });

            services.AddSingleton<IHostInfo, HostInfo>();

            services.AddScoped(typeof(IInfiniteRetryPolicy<>), typeof(InfiniteRetryPolicy<>));

            services.AddScoped(typeof(IBackOffRetryPolicy<>), typeof(BackOffRetryPolicy<>));

            services.AddScoped<CompareTimes>();

            services.AddScoped<IRouteTimeProvider, RouteTimeProviderClient>();

            services.AddScoped<IDataStreamWriteModel, MassTransitRabbitMqClient>();

            services.AddScoped<IBusInfoProvider, StmClient>();
        }

        private static void ConfigureMassTransit(IServiceCollection services)
        {
            var hostInfo = new HostInfo();
            
            var routingData = RestController.GetAddress(hostInfo.GetMQServiceName(), LoadBalancingMode.RoundRobin).Result.First();

            var uniqueQueueName = $"time_comparison.node_controller-to-any.query.{Guid.NewGuid()}";

            services.AddMassTransit(x =>
            {
                x.AddConsumer<TripComparatorMqController>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host($"rabbitmq://{ routingData.Host }:{routingData.Port}", c =>
                    {
                        c.RequestedConnectionTimeout(100);
                        c.Heartbeat(TimeSpan.FromMilliseconds(50));
                        c.PublisherConfirmation = true;
                    });

                    cfg.Message<BusPositionUpdated>(topologyConfigurator => topologyConfigurator.SetEntityName("bus_position_updated"));
                    cfg.Message<CoordinateMessage>(topologyConfigurator => topologyConfigurator.SetEntityName("coordinate_message"));

                    cfg.ReceiveEndpoint(uniqueQueueName, endpoint =>
                    {
                        endpoint.ConfigureConsumeTopology = false;

                        endpoint.Bind<CoordinateMessage>(binding =>
                        {
                            binding.ExchangeType = ExchangeType.Topic;
                            binding.RoutingKey = "trip_comparison.query";
                        });

                        endpoint.ConfigureConsumer<TripComparatorMqController>(context);
                    });

                    cfg.Publish<BusPositionUpdated>(p => p.ExchangeType = ExchangeType.Topic);
                });
            });
        }
    }
}