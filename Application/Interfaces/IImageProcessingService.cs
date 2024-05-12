using Application.REPR;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IImageProcessingService
    {
        Task<ImageProcessResponse> ProcessImage(ImageProcessRequest file, CancellationToken cancellationToken);
    }
}