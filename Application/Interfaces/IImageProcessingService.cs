using Application.CSharp.Models;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IImageProcessingService
    {
        Task<ImageProcessResponse> ProcessImage(IFormFile file, CancellationToken cancellationToken);
    }
}