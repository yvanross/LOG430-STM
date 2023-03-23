using ApplicationLogic.Usecases;
using NodeController.External.Docker;
using NodeController.External.Repository;

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

            ScheduleRecurringTasks();

            app.Run();
        }

        private static void ScheduleRecurringTasks()
        {
            var writeModel = new PodWriteModel();
            var readModel = new PodReadModel(HostInfo.ServiceAddress);
            var environmentClient = new LocalDockerClient(null);

            var servicePool = new ServicePoolDiscoveryUC(writeModel, readModel, environmentClient);

            var monitor = new MonitorUc(environmentClient, readModel, writeModel);

            readModel.GetScheduler().TryAddTask(nameof(servicePool.DiscoverServices), servicePool.DiscoverServices);
            readModel.GetScheduler().TryAddTask(nameof(monitor.GarbageCollection), monitor.GarbageCollection);
            readModel.GetScheduler().TryAddTask(nameof(monitor.ProcessPodStates), monitor.ProcessPodStates);
            readModel.GetScheduler().TryAddTask(nameof(monitor.MatchInstanceDemandOnPods), monitor.MatchInstanceDemandOnPods);
        }
    }
}