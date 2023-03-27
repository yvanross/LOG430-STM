using ApplicationLogic.Usecases;
using NodeController.External.Dao;
using NodeController.External.Docker;
using NodeController.External.Ingress;

namespace NodeController
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

            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();
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

            var logger = app.Services.GetRequiredService<ILogger<SchedulingService>>();

            _ = new SchedulingService(logger);

            app.Run();
        }

        public class SchedulingService
        {
            private readonly ILogger<SchedulingService> _logger;

            public SchedulingService(ILogger<SchedulingService> logger)
            {
                _logger = logger;
                ScheduleRecurringTasks();
            }

            private void ScheduleRecurringTasks()
            {
                _logger.LogInformation("# Schedule Recurring Tasks #");
                
                var ingressUc = new IngressUC(new HostInfo(), new IngressClient());

                ingressUc.Register().Wait();

                ingressUc.GetLogStoreAddressAndPort().Wait();

                var writeModel = new PodWriteModel();
                var readModel = new PodReadModel();
                var environmentClient = new LocalDockerClient(null);

                var servicePool = new ServicePoolDiscoveryUC(writeModel, readModel, environmentClient);

                var monitor = new MonitorUc(environmentClient, readModel, writeModel);

                readModel.GetScheduler().SetLogger(_logger);

                _logger.LogInformation("# Preparation Complete, scheduling... #");

                readModel.GetScheduler().TryAddTask(nameof(servicePool.DiscoverServices), servicePool.DiscoverServices);
                readModel.GetScheduler().TryAddTask(nameof(monitor.GarbageCollection), monitor.GarbageCollection);
                readModel.GetScheduler().TryAddTask(nameof(monitor.RemoveOrReplaceDeadPodsFromModel), monitor.RemoveOrReplaceDeadPodsFromModel);
                readModel.GetScheduler().TryAddTask(nameof(monitor.MatchInstanceDemandOnPods), monitor.MatchInstanceDemandOnPods);

                _logger.LogInformation($"# Tasks: {nameof(servicePool.DiscoverServices)}, {nameof(monitor.GarbageCollection)}, " +
                                       $"{nameof(monitor.RemoveOrReplaceDeadPodsFromModel)}, {nameof(monitor.MatchInstanceDemandOnPods)}; have been scheduled #");
            }
        }
    }
}