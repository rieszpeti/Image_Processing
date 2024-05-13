using Image_Processing_Backend.Endpoints;
using WebApi.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.SetupSwagger();

// docker run --rm -it -p 18888:18888 -p 4317:18889 -d --name aspire-dashboard mcr.microsoft.com/dotnet/nightly/aspire-dashboard:8.0.0-preview.6
// docker->logs->Login to the dashboard at http://0.0.0.0:18888/login?t=yourToken
builder.SetupOpenTelemetry();

builder.SetupSerilogLogger();

builder.SetupLayers();

var app = builder.Build();

app.SetupDevelopmentMode();

app.UseHttpsRedirection();

//Map endpoints
app.MapImageProcessingEndpoints();

app.Run();
