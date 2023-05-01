
using AuthService.Dao;
using AuthService.Services;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService
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

            builder.Services.AddDbContext<UsersContext>();

            builder.Services
                .AddIdentityCore<LabUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 0;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                })
                .AddEntityFrameworkStores<UsersContext>();

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<JwtService>();

            var app = builder.Build();

            EnsureDatabaseCreated(app);

            app.UseSwagger();

            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseCors();

            app.UseAuthentication();

            app.MapControllers();

            app.Run();
        }

        private static void EnsureDatabaseCreated(IHost app)
        {
            using var serviceScope = app.Services.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<UsersContext>();
            dbContext.Database.Migrate();
        }
    }
}