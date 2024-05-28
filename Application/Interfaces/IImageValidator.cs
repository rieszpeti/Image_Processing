using Microsoft.AspNetCore.Http;

namespace Application.CSharp.Interfaces
{
    public interface IImageValidator
    {
        void ValidateEncoding(byte[] byteArray, string encoding, CancellationToken cancellationToken);
        Task ValidateImageAsync(IFormFile file, CancellationToken cancellationToken);
    }
}