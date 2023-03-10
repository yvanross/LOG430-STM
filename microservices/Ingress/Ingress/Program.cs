
using Ambassador;
using Entities.BusinessObjects;
using Ingress.Repository;

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

            CreateDefaultRoutes();

            app.Run();
        }

        private static void CreateDefaultRoutes()
        {
            var writeModel = new RepositoryWrite();

            writeModel.UpdateServiceType(new ServiceInstance()
            {
                Address = "api.tomtom.com",
                ContainerInfo = new ContainerInfo()
                {
                    Id = "tomtom",
                    ImageName = "noImage",
                    Name = "TomtomService",
                    Port = string.Empty,
                    Status = "notAContainer"
                },
                Id = Guid.NewGuid(),
                IsHttp = false,
                Type = ServiceTypes.Tomtom.ToString()
            }, default);
        }
    }
}