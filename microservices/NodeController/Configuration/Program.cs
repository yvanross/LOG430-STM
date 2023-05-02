using ApplicationLogic.Interfaces;
using ApplicationLogic.Services;
using ApplicationLogic.Usecases;
using Infrastructure.Dao;
using Infrastructure.Docker;
using Infrastructure.Ingress;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NodeController.Controllers;
using System.Net.Mail;
using ApplicationLogic.Converters;
using Entities.DomainInterfaces.ResourceManagement;
using HostInfo = Configuration.HostInfo;
using ApplicationLogic.Interfaces.Dao;
using Monitor = ApplicationLogic.Usecases.Monitor;
using MqContracts;
using RabbitMQ.Client;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Hosting;

namespace Configuration
{
    public class Program
    {
        public static void Main(string[] args)
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

            var scheduling = app.Services.GetRequiredService<TaskScheduling>();

            scheduling.ScheduleRecurringTasks();

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            ConfigureMassTransit(services);

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new ChaosConfigDictionaryConverter());
            }).PartManager.ApplicationParts.Add(new AssemblyPart(typeof(DebugController).Assembly));

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen();

            ApplicationLogic(services);

            Infrastructure(services);

            services.AddSingleton<TaskScheduling>();

            static void ApplicationLogic(IServiceCollection services)
            {
                services.AddSingleton<IScheduler, SchedulerService>();

                services.AddScoped<ChaosExperiment>();
                
                services.AddScoped<ServicePoolDiscovery>();

                services.AddScoped<ExperimentMonitoring>();

                services.AddScoped<Ingress>();

                services.AddScoped<Monitor>();

                services.AddScoped<Routing>();

                services.AddScoped<ResourceManagementService>();
            }

            static void Infrastructure(IServiceCollection services)
            {
                services.AddSingleton<IHostInfo, HostInfo>();
                
                services.AddSingleton<IMqConfigurator, MassTransitRabbitMq>();

                services.AddScoped<ISystemStateWriteService, InfluxDbWriteService>();

                services.AddScoped<IDataStreamService, MassTransitRabbitMqClient>();

                services.AddScoped<IPodReadService, PodReadService>();

                services.AddScoped<IPodWriteService, PodWriteService>();

                services.AddScoped<IEnvironmentClient, LocalDockerClient>();

                services.AddScoped<IIngressClient, IngressClient>();
            }
        }

        private static void ConfigureMassTransit(IServiceCollection services)
        {
            var hostInfo = new HostInfo();

            var reformattedAddress = $"rabbitmq://{hostInfo.GetIngressAddress()}:{hostInfo.GetBridgePort()}";

            var baseQueueName = $"ingress_to_{hostInfo.GetTeamName()}_{hostInfo.GetUsername()}_node_controller.command";

            var uniqueQueueName = $"{baseQueueName}.{Guid.NewGuid()}";

            services.AddMassTransit(x =>
            {
                x.AddConsumer<ExperimentMqController>();
                x.AddConsumer<BusPositionUpdatedMqController>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(reformattedAddress);

                    cfg.Message<ExperimentDto>(m => m.SetEntityName("begin_experiment"));

                    cfg.ReceiveEndpoint(uniqueQueueName, endpoint =>
                    {
                        endpoint.ConfigureConsumeTopology = false;

                        endpoint.Bind<ExperimentDto>(binding =>
                        {
                            binding.ExchangeType = ExchangeType.Topic;
                            binding.RoutingKey = hostInfo.GetUsername();
                        });

                        endpoint.ConfigureConsumer<ExperimentMqController>(context);
                    });
                });
            });
        }
    }
}