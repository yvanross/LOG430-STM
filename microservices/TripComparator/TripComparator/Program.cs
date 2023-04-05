using Ambassador.Controllers;
using Ambassador;
using MassTransit;
using MassTransit.RabbitMqTransport;
using TripComparator.Controllers;
using TripComparator.DTO;
using TripComparator.External;
using HostInfo = TripComparator.External.HostInfo;
using MassTransit.Serialization;
using MqContracts;
using RabbitMQ.Client;

namespace TripComparator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "AllowOrigin",
                    builder =>
                    {
                        builder.WithOrigins("*")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            ConfigureMassTransit(builder.Services).Wait();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseCors();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static async Task ConfigureMassTransit(IServiceCollection services)
        {
            var mq = (await RestController.GetAddress(HostInfo.MqServiceName, LoadBalancingMode.RoundRobin)).FirstOrDefault();

            if (mq is not null)
            {
                var reformattedAddress = $"rabbitmq{mq.Address[4..]}";

                const string baseQueueName = "time_comparison.node_controller-to-any.query";

                var uniqueQueueName = $"{baseQueueName}.{Guid.NewGuid()}";

                services.AddMassTransit(x =>
                {
                    x.AddConsumer<TripComparatorMqController>();

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(reformattedAddress);

                        cfg.Message<BusPositionUpdated>(topologyConfigurator => topologyConfigurator.SetEntityName("bus_position_updated"));
                        cfg.Message<CoordinateMessage>(m => m.SetEntityName("coordinate_message"));

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
                        
                        cfg.Publish<BusPositionUpdated>(p=>p.ExchangeType = ExchangeType.Topic);
                    });
                });
            }
        }
    }
}