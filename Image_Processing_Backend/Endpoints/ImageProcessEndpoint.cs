using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace Image_Processing_Backend.Endpoints
{
    public static class ImageProcessEndpoint
    {
        public static void MapImageProcessingEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("imageProcess", async (
            ILogger logger,
            IFormFile file,
            IImageProcessingService service,
            CancellationToken ct = default) =>
            {
                logger.LogDebug("Incoming request for image processing.");

                var response = await service.ProcessImage(file, ct);

                logger.LogInformation("Image processing completed successfully.");

                return Results.File(response.bytes, $"image/{response.FileExtension}");
            })
            .DisableAntiforgery(); // unsafe, setup Antiforgery in PROD
        }
    }
}
