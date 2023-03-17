using ApplicationLogic.Usecases;
using NodeController.Docker;
using NodeController.Repository;

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

            var monitor = new MonitorUc(new LocalDockerClient(null), new RepositoryRead(HostInfo.ServiceAddress), new RepositoryWrite());

            monitor.TryScheduleStateProcessingOnScheduler();

            app.Run();
        }
    }
}