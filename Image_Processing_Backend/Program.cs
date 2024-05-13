using Image_Processing_Backend.Endpoints;
using WebApi.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.SetupSwagger();

builder.SetupOpenTelemetry();

//builder.SetupLogger();

builder.SetupLayers();

var app = builder.Build();

app.SetupDevelopmentMode();

app.UseHttpsRedirection();

//Map endpoints
app.MapImageProcessingEndpoints();

app.Run();
