using System.Runtime.InteropServices;
using System.Threading.Channels;
using Application.CSharp.Interfaces;
using Application.CSharp.Models;
using Application.CSharp.ModelValidation;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using static Application.CSharp.Services.CPPAdapter;

namespace Application.Services
{
    /// <summary>
    /// this class is highly dependent on CPPAdapter and uses C++ function to process image
    /// </summary>
    public class ImageProcessingService : IImageProcessingService
    {
        private readonly ILogger _logger;
        private readonly IImageValidator _imageValidator;

        public ImageProcessingService(ILogger logger, IImageValidator imageValidator)
        {
            _logger = logger;
            _imageValidator = imageValidator;
        }

        /// <summary>
        /// Steps of this function:
        /// 1. Validate image
        /// 2. call the C++ method and gets back an image via pointer
        /// 3. validate the manipulated image in byte format
        /// 4. returns the result object
        /// </summary>
        /// <param name="file"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ImageProcessResponse> ProcessImage(IFormFile file, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Start image process in service: {name}", nameof(ImageProcessingService));

                await _imageValidator.ValidateImageAsync(file, cancellationToken);

                _logger.LogDebug("Image validation successful filename: {filename}", file.Name);

                byte[] imageData;
                byte[] result;

                ImageInfo imInfo = new ImageInfo();

                var extension = Path.GetExtension(file.FileName);

                cancellationToken.ThrowIfCancellationRequested();

                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream, cancellationToken);
                    imageData = memoryStream.ToArray();
                    result = new byte[imageData.Length];

                    _logger.LogDebug("Start C++ call.");

                    IntPtr ptr = ProcessImageCpp(
                                    imageData,
                                    memoryStream.Length,
                                    extension,
                                    ref imInfo);

                    byte[] imagePixels = new byte[imInfo.size];
                    Marshal.Copy(imInfo.data, imagePixels, 0, imInfo.size);

                    _logger.LogInformation("C++ function succesfully completed.");

                    if (imInfo.data != IntPtr.Zero)
                    {
                        ReleaseMemoryFromC(imInfo.data);
                    }
                    else
                    {
                        _logger.LogWarning("Release memory unsuccessful!");
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    _imageValidator.ValidateEncoding(imagePixels, cancellationToken);

                    _logger.LogDebug("Image validation successful.");

                    return new ImageProcessResponse
                    {
                        Bytes = imagePixels,
                        FileExtension = extension.Replace(".", string.Empty)
                    };
                }
            }
            catch (SEHException ex)
            {
                _logger.LogCritical("Could not process image: {msg}", ex.Message);
                throw;
            }
            catch (OperationCanceledException cancel)
            {
                _logger.LogInformation($"Cancelled: {cancel}");
                throw;
            }
        }
    }
}
