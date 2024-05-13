using System.Runtime.InteropServices;
using Application.CSharp.Models;
using Application.CSharp.ModelValidation;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using static Application.CSharp.Services.CPPAdapter;

namespace Application.Services
{
    public class ImageProcessingService : IImageProcessingService
    {
        private readonly ImageValidator _imageValidator;

        public ImageProcessingService(ImageValidator imageValidator)
        {
            _imageValidator = imageValidator;
        }

        public async Task<ImageProcessResponse> ProcessImage(IFormFile file, CancellationToken cancellationToken)
        {

            await _imageValidator.ValidateImageAsync(file, cancellationToken);

            byte[] imageData;
            byte[] result;

            ImageInfo imInfo = new ImageInfo();
            MemoryStream convertedImageMemoryStream;

            var extension = Path.GetExtension(file.FileName);

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                imageData = memoryStream.ToArray();
                result = new byte[imageData.Length];

                IntPtr ptr = ProcessImageCpp(
                                imageData,
                                memoryStream.Length,
                                extension,
                                ref imInfo);

                byte[] imagePixels = new byte[imInfo.size];
                Marshal.Copy(imInfo.data, imagePixels, 0, imInfo.size);

                if (imInfo.data != IntPtr.Zero)
                {
                    ReleaseMemoryFromC(imInfo.data);
                }

                _imageValidator.ValidateEncoding(imagePixels, cancellationToken);

                return new ImageProcessResponse
                {
                    bytes = imagePixels,
                    FileExtension = extension.Replace(".", string.Empty)
                };
            }
        }
    }
}
