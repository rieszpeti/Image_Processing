using Application.CSharp.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using static Application.CSharp.ModelValidation.ValidatorOptions;

namespace Application.CSharp.ModelValidation
{
    public class ImageValidator : IImageValidator
    {
        private readonly ILogger _logger;

        public ImageValidator(ILogger logger)
        {
            _logger = logger;
        }

        public async Task ValidateImageAsync(IFormFile file, CancellationToken cancellationToken)
        {
            if (file is null || file.Length == 0)
            {
                _logger.LogInformation("File is null or empty!");
                throw new ArgumentNullException("The file cannot be null or empty.");
            }

            cancellationToken.ThrowIfCancellationRequested();

            var extension = Path.GetExtension(file.FileName).ToLower();
            var encodingType = extension switch
            {
                ".jpg" => EncodingType.JPG,
                ".png" => EncodingType.PNG,
                _ => throw new ArgumentException("Invalid file extension.")
            };

            _logger.LogInformation("File extension: {extension}", encodingType);

            cancellationToken.ThrowIfCancellationRequested();

            var maxFileSize = 5 * 1024 * 1024; // 5 MB
            if (file.Length > maxFileSize)
            {
                _logger.LogInformation("File is bigger than limit: {actualSize}", file.Length);
                throw new ArgumentException("File size exceeds the maximum limit.");
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Infer an Image type by looking at the first four bytes of a raw byte array
        /// Based on: https://stackoverflow.com/questions/210650/validate-image-from-file-in-c-sharp
        /// Based on: https://gist.github.com/markcastle/3cc99c8e5756c7e27532900a5f8a2a93
        /// I've checked png, jpeg
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns>EncodingType</returns>
        public void ValidateEncoding(byte[] byteArray, CancellationToken cancellationToken)
        {
            const int INT_SIZE = 4; // We only need to check the first four bytes of the file / byte array.

            // Copy the first 4 bytes into our buffer 
            var buffer = new byte[INT_SIZE];
            System.Buffer.BlockCopy(byteArray, 0, buffer, 0, INT_SIZE);

            cancellationToken.ThrowIfCancellationRequested();

            if (png.SequenceEqual(buffer.Take(png.Length)))
            {
                _logger.LogInformation("Fileformat: {format}", nameof(png));
            }
            else if (jpg.SequenceEqual(buffer.Take(jpg.Length)))
            {
                _logger.LogInformation("Fileformat: {format}", nameof(jpg));
            }
            else
            {
                throw new ArgumentException("Invalid image format.");
            }
        }

    }
}
