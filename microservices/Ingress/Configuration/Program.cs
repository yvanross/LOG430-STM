using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Dao;
using ApplicationLogic.Services;
using ApplicationLogic.Usecases;
using Entities.DomainInterfaces;
using Infrastructure.Clients;
using Infrastructure.Dao;
using Infrastructure.Repository;
using Ingress.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using MassTransit;
using Monitor = ApplicationLogic.Usecases.Monitor;
using MqContracts;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

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

            app.MapControllers();

            var scheduler = app.Services.GetRequiredService<SchedulingTask>();

            scheduler.ScheduleRecurringTasks();

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            ConfigureMassTransit(services);

            services.AddCors();

            services.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(typeof(IngressController).Assembly));

            services.AddHttpContextAccessor();

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c => c.EnableAnnotations());

            ApplicationLogic(services);

            Infrastructure(services);

            services.AddSingleton<SchedulingTask>();

            static void ApplicationLogic(IServiceCollection services)
            {
                services.AddSingleton<IScheduler, Scheduler>();

                services.AddScoped<Monitor>();

                services.AddScoped<Subscription>();
            }

            static void Infrastructure(IServiceCollection services)
            {
                services.AddSingleton<IHostInfo, HostInfo>();

                services.AddScoped<IAuthorizationService, AuthorizationServiceClient>();

                services.AddScoped<ISystemStateReadService, InfluxDbReadService>();

                services.AddScoped<INodeControllerClient, NodeControllerClient>();

                services.AddScoped<IRepositoryRead, RepositoryRead>();

                services.AddScoped<IRepositoryWrite, RepositoryWrite>();
                
                services.AddScoped<IDataStream, MassTransitRabbitMqClient<ExperimentDto>>();
            }
        }

        private static void ConfigureMassTransit(IServiceCollection services)
        {
            var hostInfo = new HostInfo();

            var reformattedAddress = $"rabbitmq://{hostInfo.GetAddress()}:{hostInfo.GetBridgePort()}";

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((_, cfg) =>
                {
                    cfg.Host(reformattedAddress);

                    cfg.Message<ExperimentDto>(topologyConfigurator => topologyConfigurator.SetEntityName("begin_experiment"));

                    cfg.Publish<ExperimentDto>(p => p.ExchangeType = ExchangeType.Topic);
                });
            });
        }
    }
    
}