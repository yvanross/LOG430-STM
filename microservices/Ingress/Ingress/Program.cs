
using ApplicationLogic.Usecases;
using Entities.BusinessObjects;
using Ingress.Repository;
using NuGet.Packaging.Signing;

namespace Ingress
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
            var readModel = new RepositoryRead();

            var monitor = new MonitorUc(readModel, new RepositoryWrite());

            readModel.GetScheduler().TryAddTask(monitor.BeginProcessingHeartbeats);
        }
    }
}