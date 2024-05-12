using Application.Interfaces;
using Application.REPR;
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
            IFormFile file,
            IImageProcessingService service,
            CancellationToken ct = default) =>
            {
                var image = await service.ProcessImage(new ImageProcessRequest { File = file }, ct);

                return Results.File(image.bytes, $"image/{image.FileExtension}");
            })
            .DisableAntiforgery();
        }
    }
}
