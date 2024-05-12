using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Application.CSharp.Interfaces;
using Application.Interfaces;
using Application.REPR;
using Microsoft.AspNetCore.Http;
using static System.Net.Mime.MediaTypeNames;

namespace Application.Services
{
    public class ImageProcessingService : IImageProcessingService
    {
        private readonly IModelValidator _modelValidator;

        public ImageProcessingService(IModelValidator modelValidator)
        {
            _modelValidator = modelValidator;
        }

        public struct ImageInfo
        {
            public IntPtr data;
            public int size;
        }

        private const string dllPath = @"..\..\..\..\x64\Debug\Application.CPP.dll";

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern void ReleaseMemoryFromC(IntPtr buf);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr ProcessImageCpp(
            byte[] img,
            long data_len,
            string fileExtension,
            ref ImageInfo imTemplate);

        public async Task<ImageProcessResponse> ProcessImage(ImageProcessRequest request, CancellationToken cancellationToken)
        {
            var (isValid, errorMessage) = _modelValidator.Validate(request);

            if (!isValid)
            {
                if (errorMessage is null)
                {
                    throw new ArgumentNullException(nameof(errorMessage));
                }
                return new ImageProcessResponse
                {
                    Image = null,
                    IsSuccess = false,
                    Message = errorMessage
                };
            }

            //var errors = new List<ValidationResult>();
            //var isValid = Validator.TryValidateObject(request, new ValidationContext(request), errors, true);

            //if (!isValid)
            //{
            //    StringBuilder errorMessage = new StringBuilder();
            //    foreach (var error in errors)
            //    {
            //        errorMessage.Append(error.ErrorMessage).Append(" ");
            //    }

            //    return new ImageProcessResponse
            //    {
            //        Image = null,
            //        IsSuccess = false,
            //        Message = errorMessage.ToString()
            //    };
            //}

            byte[] imageData;
            byte[] result;

            ImageInfo imInfo = new ImageInfo();
            MemoryStream convertedImageMemoryStream;

            var extension = Path.GetExtension(request.File.FileName);

            using (var memoryStream = new MemoryStream())
            {
                await request.File.CopyToAsync(memoryStream);
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
                convertedImageMemoryStream = new MemoryStream(imagePixels);

                return new ImageProcessResponse
                {
                    Image = null,
                    IsSuccess = true,
                    Message = string.Empty,
                    bytes = imagePixels,
                    FileExtension = extension.Replace(".", string.Empty)
                };
            }
        }
    }
}
