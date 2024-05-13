using Application.CSharp.Models;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Image_Processing_Backend.Endpoints
{
    public static class ImageProcessEndpoint
    {
        public static void MapImageProcessingEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("imageProcess", ProcessImage)
            .DisableAntiforgery(); // unsafe, setup Antiforgery in PROD
        }

        public static async Task<IResult> ProcessImage(
            ILogger logger, IFormFile file, IImageProcessingService service, CancellationToken ct = default)
        {
            try
            {
                logger.LogDebug("Incoming request for image processing.");

                using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct))
                {
                    var response = await service.ProcessImage(file, linkedCts.Token);

                    logger.LogInformation("Image processing completed successfully.");

                    return Results.File(response.Bytes, $"image/{response.FileExtension}");
                }
            }
            catch (OperationCanceledException e)
            {
                logger.LogInformation($"{nameof(OperationCanceledException)} thrown with message: {e.Message}");
                return Results.Ok("Operation canceled while processing the image.");
            }
            catch (ArgumentException e)
            {
                logger.LogWarning($"{nameof(ArgumentException)} thrown with message: {e.Message}");
                return Results.Problem(
                    title: "An error occurred while processing the image.",
                    detail: e.Message);
            }
            catch (Exception ex)
            {
                logger.LogError($"Image processing failed: {ex.Message}");
                return Results.Problem("An error occurred while processing the image.");
            }
        }
    }
}
