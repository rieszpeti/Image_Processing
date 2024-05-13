using Application;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

namespace WebApi.Startup
{
    /// <summary>
    /// Extension class to clean up the Program.cs for better readability
    /// </summary>
    internal static class StartupExtensions
    {
        public static void SetupSwagger(this IHostApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Image Processing API",
                    Description = "An ASP.NET Core Web API for Image Processing",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Peter Riesz",
                        Url = new Uri("https://example.com/contact")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Free to use",
                        Url = new Uri("https://example.com/license")
                    }
                });
            });
        }

        public static void SetupSerilogLogger(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration));

            builder.Services.AddSingleton(sp => sp.GetRequiredService<ILoggerFactory>().CreateLogger("DefaultLogger"));
        }

        public static void SetupOpenTelemetry(this WebApplicationBuilder builder)
        {
            builder.Logging.AddOpenTelemetry(x => 
            {
                x.IncludeScopes = true;
                x.IncludeFormattedMessage = true;   
            });

            builder.Services.AddOpenTelemetry()
                .WithMetrics(x =>
                {
                    x.AddRuntimeInstrumentation()
                        .AddMeter(
                            "Microsoft.AspNetCore.Hosting",
                            "Microsoft.AspNetCore.Server.Kestrel",
                            "System.Net.Http",
                            "ImageProcess.Api"
                        );
                })
                .WithTracing(x =>
                {
                    if (builder.Environment.IsDevelopment())
                    {
                        x.SetSampler<AlwaysOnSampler>();
                    }

                    x.AddAspNetCoreInstrumentation()
                     .AddHttpClientInstrumentation();
                });

            builder.Services.Configure<OpenTelemetryLoggerOptions>(logging => logging.AddOtlpExporter());
            builder.Services.ConfigureOpenTelemetryMeterProvider(metrics => metrics.AddOtlpExporter());
            builder.Services.ConfigureOpenTelemetryTracerProvider(tracing => tracing.AddOtlpExporter());

            builder.Services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

            builder.Services.ConfigureHttpClientDefaults(http =>
            {
                http.AddStandardResilienceHandler();
            });

            builder.Services.AddMetrics();
            builder.Services.AddSingleton<ImageProcessMetrics>();
        }

        //// enum serialization for minimal api
        //// https://stackoverflow.com/questions/76643787/how-to-make-enum-serialization-default-to-string-in-minimal-api-endpoints-and-sw
        //public static void SetupMinimalApiEnumSupport(this WebApplicationBuilder builder)
        //{
        //    builder.Services.ConfigureHttpJsonOptions(options =>
        //    {
        //        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        //    });
        //    builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
        //    {
        //        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        //    });
        //}

        public static void SetupDevelopmentMode(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
        }

        /// <summary>
        /// Adding services from different layers from the architecture
        /// for example you can add infrastructure where will be dapper or entity framework core
        /// </summary>
        /// <param name="builder"></param>
        public static void SetupLayers(this IHostApplicationBuilder builder)
        {
            builder.Services
                .AddApplication();
                //.AddInfrastructure();
        }
    }
}
