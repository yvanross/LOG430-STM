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

            
            BeginScheduler();

            app.Run();
        }

        private static void BeginScheduler()
        {
            var writeModel = new PodWriteModel();
            var readModel = new PodReadModel(HostInfo.ServiceAddress);
            var environmentClient = new LocalDockerClient(null);


            var monitor = new MonitorUc(environmentClient, readModel, writeModel);

            monitor.TryScheduleStateProcessingOnScheduler();

            var _servicePool = new ServicePoolDiscoveryUC(writeModel, readModel, environmentClient);
        }
    }
}