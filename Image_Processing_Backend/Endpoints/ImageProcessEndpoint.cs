using Application.Interfaces;
using Application.REPR;
using Microsoft.AspNetCore.Mvc;

namespace Image_Processing_Backend.Endpoints
{
    public static class ImageProcessEndpoint
    {
        public static void MapImageProcessingEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("imageProcess", async (
            ImageProcessRequest request,
            IImageProcessingService service,
            CancellationToken ct = default) =>
            {
                var image = await service.ProcessImage(request);

                // Base64 string dekódolása byte tömbbé
                byte[] imageBytes = Convert.FromBase64String(request.Image);

                return Results.Ok(new FileContentResult(imageBytes, "image/png"));
            });
        }
    }
}
