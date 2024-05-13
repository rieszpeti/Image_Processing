using Microsoft.AspNetCore.Http;

namespace Application.CSharp.Interfaces
{
    public interface IImageValidator
    {
        void ValidateEncoding(byte[] byteArray, CancellationToken cancellationToken);
        Task ValidateImageAsync(IFormFile file, CancellationToken cancellationToken);
    }
}