
using Ambassador;
using Ambassador.Usecases;

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

            var logger = app.Services.GetRequiredService<ILogger<AmbassadorService>>();

            _ = new AmbassadorService(logger);

            app.Run();
        }
    }

    public class AmbassadorService
    {
        private readonly RegistrationUC _registration = new();

        public AmbassadorService(ILogger<AmbassadorService> logger)
        {
            _registration.Register(ServiceTypes.ComparateurTrajet.ToString(), logger);
        }
    }
}