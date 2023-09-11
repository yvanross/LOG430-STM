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
using Entities.Dao;
using Infrastructure.L4ConnectionListener;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NodeController.Controllers.Mq;
using Moq;

namespace Configuration
{
    public class Program
    {
        private static readonly HostInfo HostInfo = new ();

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.UseKestrel(options =>
            {
                foreach (var port in HostInfo.GetTunnelPortRange())
                {
                    options.ListenAnyIP(port, listenOptions =>
                    {
                        listenOptions.UseConnectionHandler<L4LoadBalancer>();
                    });
                }

                options.ListenAnyIP(80);
            });

            ConfigureServices(builder.Services);

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

            var scheduling = app.Services.GetRequiredService<TaskScheduling>();

            scheduling.ScheduleRecurringTasks().Wait();

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            ConfigureMassTransit(services);

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new ChaosConfigDictionaryConverter());
            }).PartManager.ApplicationParts.Add(new AssemblyPart(typeof(RoutingController).Assembly));

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(2, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });

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

                services.AddScoped<IRouting, Routing>();

                services.AddScoped<ResourceManagementService>();

                services.AddScoped<PlannedResourcesUpdate>();
            }

            static void Infrastructure(IServiceCollection services)
            {
                services.AddSingleton<IHostInfo, HostInfo>();
                
                services.AddSingleton<IMqConfigurator, MassTransitRabbitMq>();

                if (HostInfo.IsIngressConfigValid())
                {
                    services.AddScoped<IDataStreamService, MassTransitRabbitMqClient>();

                    services.AddScoped<IHeartbeatService, MassTransitRabbitMqClient>();

                    services.AddScoped<IIngressClient, IngressClient>();

                    services.AddScoped<L4Logger>();

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

                services.AddScoped<IEnvironmentClient, DockerdClient>();
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

            services.AddOptions<MassTransitHostOptions>()
                .Configure(options =>
                {
                    options.StopTimeout = TimeSpan.FromMilliseconds(50);
                    options.ConsumerStopTimeout = TimeSpan.FromMilliseconds(50);
                });

            services.AddMassTransit(x =>
            {
                if (hostInfo.IsIngressConfigValid() is false) return;

                x.AddConsumer<ExperimentMqController>();
                x.AddConsumer<BusPositionUpdatedMqController>();
                x.AddConsumer<AckErrorMqController>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(reformattedAddress, c =>
                    {
                        c.RequestedConnectionTimeout(1000);
                    });

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