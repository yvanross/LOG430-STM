using System.Resources;
using ApplicationLogic.Interfaces;
using ApplicationLogic.Interfaces.Policies;
using ApplicationLogic.Services.BusTracking;
using Infrastructure.External;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Infrastructure.Cache;
using ApplicationLogic.Services.Itinerary;
using ApplicationLogic.Use_Cases;
using Configuration.Properties;
using Configuration.Policies;
using Controllers.Controllers;
using ServiceMeshHelper;
using ServiceMeshHelper.Controllers;
using ServiceMeshHelper.Services;
using STMTests.Use_Cases;

namespace Configuration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder.Services);

            var app = builder.Build();

            app.UseSwagger();

            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseCors(
                options =>
                {
                    options.AllowAnyOrigin();
                    options.AllowAnyHeader();
                    options.AllowAnyMethod();
                }
            );

            app.UseAuthorization();

            app.MapControllers();

            var itinerary = app.Services.GetRequiredService<IItinerary>();

            itinerary.PrefetchAndApplyTripUpdates().Wait();

            GC.Collect();

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            //todo FinderController is used to tell which assembly contains the swagger controllers
            services.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(typeof(FinderController).Assembly));

            Trysmt().Wait();
            
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen();

            services.AddSingleton(_ => new ResourceManager(typeof(Resources)));

            services.AddScoped(typeof(IInfiniteRetryPolicy<>), typeof(InfiniteRetryPolicy<>));

            services.AddScoped(typeof(IBackOffRetryPolicy<>), typeof(BackOffRetryPolicy<>));

            ApplicationLogic(services);

            Infrastructure(services);

            services.AddScoped<GtfsProcessorService>();

            static void ApplicationLogic(IServiceCollection services)
            {
                services.AddScoped<ABusTrackingService, BeforeFirstStopTrackingService>();

                services.AddScoped<GtfsProcessorService>();

                services.AddScoped<ItineraryPlannerService>();

                services.AddScoped<IItinerary, ItineraryStub>();

                services.AddScoped<ITrackBus, TrackBusStub>();
            }

            static void Infrastructure(IServiceCollection services)
            {
                services.AddSingleton<GtfsFileFileCache>();

                services.AddSingleton<IStmClient, StmClient>();

                services.AddSingleton<ITransitDataCache, TransitDataCache>();
            }
        }

        private static async Task Trysmt()
        {
            var host = await TcpController.GetTcpSocketForRabbitMq("EventStream");

            Console.WriteLine(host);
        }
    }
}