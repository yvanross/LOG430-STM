using System.Reflection;
using GTFS;
using Microsoft.OpenApi.Models;
using STM.ExternalServiceProvider;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c => {
    
    c.SwaggerDoc("v1",
        new OpenApiInfo()
        {
            Title = "STM API",
            Description = $"To make a call to the relevant Endpoint, format the URL as follows: http://10.194.33.155.nip.io:49158/ + 'Endpoint_Name' + ? + 'params'. " +
                          $"\n example: http://10.194.33.155.nip.io:49158/STMOptimalBus?fromLatitudeLongitude=45.501782%2C%20-73.576577&toLatitudeLongitude=45.508232%2C%20-73.571325",
            Version = "1.1"
        }
    );

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

STMData.PrefetchData();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
