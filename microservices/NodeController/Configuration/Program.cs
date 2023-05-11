using ApplicationLogic.Interfaces;
using ApplicationLogic.Services;
using ApplicationLogic.Usecases;
using Infrastructure.Dao;
using Infrastructure.Docker;
using Infrastructure.Ingress;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NodeController.Controllers;
using ApplicationLogic.Converters;
using Entities.DomainInterfaces.ResourceManagement;
using ApplicationLogic.Interfaces.Dao;
using Monitor = ApplicationLogic.Usecases.Monitor;
using MqContracts;
using RabbitMQ.Client;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using NodeController.Controllers.Mq;
using NuGet.Packaging.Signing;
using Moq;

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
            }).PartManager.ApplicationParts.Add(new AssemblyPart(typeof(RoutingController).Assembly));

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen();

            ApplicationLogic(services);

            Infrastructure(services, new HostInfo());

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

                services.AddScoped<PlannedResourcesUpdate>();
            }

            static void Infrastructure(IServiceCollection services, IHostInfo hostInfo)
            {
                services.AddSingleton<IHostInfo, HostInfo>();
                
                services.AddSingleton<IMqConfigurator, MassTransitRabbitMq>();

                if (hostInfo.IsIngressConfigValid())
                {
                    services.AddScoped<IDataStreamService, MassTransitRabbitMqClient>();

                    services.AddScoped<IHeartbeatService, MassTransitRabbitMqClient>();

                    services.AddScoped<IIngressClient, IngressClient>();

                    services.AddScoped<ISystemStateWriteService, InfluxDbWriteService>();

                }
                else
                {
                    services.AddScoped(typeof(IDataStreamService), _ => GetDataStreamServiceMock());

                    services.AddScoped(typeof(IHeartbeatService), _ => GetHeartbeatServiceMock());

                    services.AddScoped(typeof(IIngressClient), _ => GetIngressClientMock());

                    services.AddScoped(typeof(ISystemStateWriteService), _ => GetInfluxDbWriteServiceMock());
                }

                services.AddScoped<IPodReadService, PodReadService>();

                services.AddScoped<IPodWriteService, PodWriteService>();

                services.AddScoped<IEnvironmentClient, LocalDockerClient>();
            }
        }

        private static IDataStreamService GetDataStreamServiceMock()
        {
            var mock = new Mock<IDataStreamService>();

            var any = It.IsAny<ICoordinates>();

            mock.Setup(e => e.Produce(any));

            return mock.Object;
        }

        private static IHeartbeatService GetHeartbeatServiceMock()
        {
            var mock = new Mock<IHeartbeatService>();

            var any = It.IsAny<HeartBeatDto>();

            mock.Setup(e => e.Produce(any));

            return mock.Object;
        }

        private static IIngressClient GetIngressClientMock()
        {
            var mock = new Mock<IIngressClient>();

            var any = It.IsAny<string>();

            mock.Setup(e => e.Subscribe(any, any, any, any, any, any));
            mock.Setup(e => e.GetLogStoreAddressAndPort(any));

            return mock.Object;
        }

        private static ISystemStateWriteService GetInfluxDbWriteServiceMock()
        {
            var mock = new Mock<ISystemStateWriteService>();

            var any = It.IsAny<IExperimentReport>();

            mock.Setup(e => e.Log(any));

            return mock.Object;
        }

        private static void ConfigureMassTransit(IServiceCollection services)
        {
            var hostInfo = new HostInfo();

            var ingressHost = hostInfo.GetIngressAddress();

            var ingressPort = hostInfo.GetBridgePort();

            var reformattedAddress = $"rabbitmq://{ingressHost}:{ingressPort}";

            var baseQueueName = $"ingress_to_{hostInfo.GetTeamName()}_{hostInfo.GetUsername()}_node_controller.command";

            var uniqueQueueName = $"{baseQueueName}.{Guid.NewGuid()}";

            services.AddMassTransit(x =>
            {
                if (hostInfo.IsIngressConfigValid() is false) return;

                x.AddConsumer<ExperimentMqController>();
                x.AddConsumer<BusPositionUpdatedMqController>();
                x.AddConsumer<AckErrorMqController>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(reformattedAddress);

                    cfg.Message<ExperimentDto>(m => m.SetEntityName("begin_experiment"));
                    
                    cfg.Message<AckErrorMqController>(m => m.SetEntityName("ack_error_event"));
                    
                    cfg.Message<HeartBeatDto>(m => m.SetEntityName("heartBeat"));
                    cfg.Publish<HeartBeatDto>(p => p.ExchangeType = ExchangeType.Topic);

                    cfg.ReceiveEndpoint(uniqueQueueName, endpoint =>
                    {
                        endpoint.QueueExpiration = TimeSpan.FromDays(2);
                        
                        endpoint.ConfigureConsumeTopology = false;

                        endpoint.Bind<ExperimentDto>(binding =>
                        {
                            binding.ExchangeType = ExchangeType.Topic;
                            binding.RoutingKey = hostInfo.GetUsername();
                        });

                        endpoint.Bind<AckErrorMqController>(binding =>
                        {
                            binding.ExchangeType = ExchangeType.Topic;
                            binding.RoutingKey = hostInfo.GetUsername();
                        });

                        endpoint.ConfigureConsumer<ExperimentMqController>(context);
                        endpoint.ConfigureConsumer<AckErrorMqController>(context);
                    });
                });
            });
        }
    }
}