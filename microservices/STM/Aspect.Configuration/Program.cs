using Application.Commands.AntiCorruption;
using Application.Queries.AntiCorruption;
using Aspect.Configuration.Dispatchers;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Aspect.Configuration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton<ICommandDispatcher, CommandDispatcher>();
            services.TryAddSingleton<IQueryDispatcher, QueryDispatcher>();

            services.Scan(selector =>
            {
                selector.FromCallingAssembly()
                    .AddClasses(filter =>
                    {
                        filter.AssignableTo(typeof(IQueryHandler<,>));
                    })
                    .AsImplementedInterfaces()
                    .WithSingletonLifetime();
            });
            services.Scan(selector =>
            {
                selector.FromCallingAssembly()
                    .AddClasses(filter =>
                    {
                        filter.AssignableTo(typeof(ICommandHandler<>));
                    })
                    .AsImplementedInterfaces()
                    .WithSingletonLifetime();
            });
        }

    }
}