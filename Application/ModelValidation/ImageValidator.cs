using Application.Models;
using Microsoft.AspNetCore.Http;
using static System.Net.Mime.MediaTypeNames;

namespace Application.CSharp.ModelValidation
{
    public class ImageValidator
    {
        public async Task ValidateImageAsync(IFormFile file, CancellationToken cancellationToken)
        {
            if (file is null || file.Length == 0)
            {
                throw new ArgumentNullException("The file cannot be null or empty.");
            }

            cancellationToken.ThrowIfCancellationRequested();

            var allowedExtensions = new string[] { ".jpg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Invalid file extension.");
            }

            cancellationToken.ThrowIfCancellationRequested();

            var maxFileSize = 5 * 1024 * 1024; // 5 MB
            if (file.Length > maxFileSize)
            {
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

            var png = new byte[] { 137, 80, 78, 71 };                // PNG
            var jpeg = new byte[] { 255, 216, 255, 224 };            // jpeg

            // Copy the first 4 bytes into our buffer 
            var buffer = new byte[INT_SIZE];
            System.Buffer.BlockCopy(byteArray, 0, buffer, 0, INT_SIZE);

            cancellationToken.ThrowIfCancellationRequested();

            if (png.SequenceEqual(buffer.Take(png.Length)))
            {
            }
            else if (jpeg.SequenceEqual(buffer.Take(jpeg.Length)))
            {
            }
            else
            {
                throw new ArgumentException("Invalid image format.");
            }
        }

    }
}
