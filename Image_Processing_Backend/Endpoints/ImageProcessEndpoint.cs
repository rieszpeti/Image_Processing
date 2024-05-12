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
                var image = await service.ProcessImage(new ImageProcessRequest { File = file });

                //file.OpenReadStream();

                //byte[] imageData;
                //using (var memoryStream = new MemoryStream())
                //{
                //    //await image.Image.CopyToAsync(memoryStream);
                //    var x = image.Image.OpenReadStream();
                //    x.CopyTo(memoryStream);
                //    imageData = memoryStream.ToArray();
                //}

                //return Results.Ok(new FileContentResult(image.Image, "image/png"));

                //return Results.Ok();

                //return new FileContentResult(image.bytes, "image/png");

                //return Results.File(imageData, "image/png");

                //var im = image.Image.OpenReadStream();

                //return Results.File(image.Image.OpenReadStream(), "image/png");

                //return Results.File(image.Image.OpenReadStream(), "image/png");
            })
            .DisableAntiforgery();
        }
    }
}
