using Image_Processing_Backend.Endpoints;
using WebApi.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.SetupSwagger();

// you can get the brand new Aspire dashboard, if you run this on docker
// more about here: https://www.youtube.com/watch?v=A2pKhNQoQUU&t=400s
// docker run --rm -it -p 18888:18888 -p 4317:18889 -d --name aspire-dashboard mcr.microsoft.com/dotnet/nightly/aspire-dashboard:8.0.0-preview.6
// docker->logs->Login to the dashboard at http://0.0.0.0:18888/login?t=yourToken
builder.SetupOpenTelemetry();

// for basic file logging and console log
builder.SetupSerilogLogger();

// If this would be an N layer architecture or Clean Architecture,
// then you would add here the layers like Domain or infrastructure
builder.SetupLayers();

var app = builder.Build();

app.SetupDevelopmentMode();

app.UseHttpsRedirection();

//Map minimal endpoints
app.MapImageProcessingEndpoints();

app.Run();
