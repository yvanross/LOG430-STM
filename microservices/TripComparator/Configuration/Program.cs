using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Policies;
using ApplicationLogic.Usecases;
using Configuration.Policies;
using Controllers.Controllers;
using Entities.DomainInterfaces;
using Infrastructure.Clients;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MqContracts;
using Polly;
using RabbitMQ.Client;
using ServiceMeshHelper.Controllers;
using HostInfo = TripComparator.External.HostInfo;

namespace Configuration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Todo tactique disponibilité retry
            Policy.Handle<Exception>().RetryForever().Execute(() =>
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

                app.Run();
            });
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            ConfigureMassTransit(services).Wait();

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

        private static async Task ConfigureMassTransit(IServiceCollection services)
        {
            await Task.Delay(5_000);

            var host = await TcpController.GetTcpSocketForSericeType(HostInfo.MqServiceName);

            const string baseQueueName = "time_comparison.node_controller-to-any.query";

            var uniqueQueueName = $"{baseQueueName}.{Guid.NewGuid()}";

            services.AddMassTransit(x =>
            {
                x.AddConsumer<TripComparatorMqController>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(host);

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
                        
                        endpoint.SetQuorumQueue();
                    });
                    
                    cfg.Publish<BusPositionUpdated>(p =>
                        p.ExchangeType = ExchangeType.Topic);
                });
            });
        }
    }
}