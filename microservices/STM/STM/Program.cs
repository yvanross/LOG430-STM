using System.Reflection;
using Ambassador;
using Ambassador.Controllers;
using GTFS;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using STM.ExternalServiceProvider;

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

builder.Services.AddSwaggerGen(c => {
    
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

new StmData().PrefetchData();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();
app.MapControllers();

var logger = app.Services.GetRequiredService<ILogger<AmbassadorService>>();

_ = new AmbassadorService(logger);

app.Run();

public class AmbassadorService
{
    public AmbassadorService(ILogger<AmbassadorService> logger)
    {
        RegistrationController.Register(ServiceTypes.ComparateurTrajet.ToString(), logger);
    }
}