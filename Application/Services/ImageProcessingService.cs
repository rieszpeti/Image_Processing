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
using Application.Interfaces;
using Application.REPR;
using Microsoft.AspNetCore.Http;
using static System.Net.Mime.MediaTypeNames;

namespace Application.Services
{
    public class ImageProcessingService : IImageProcessingService
    {
        public struct ImageInfo
        {
            public IntPtr data;
            public int size;
        }

        private const string dllPath = @"..\..\..\..\x64\Debug\Application.CPP.dll";

        [DllImport(dllPath, /*ExactSpelling = false,*/ CallingConvention = CallingConvention.Cdecl)]
        static extern void ReleaseMemoryFromC(IntPtr buf);

        //[DllImport(@"CDll2.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl/*, CharSet = CharSet.Ansi)*/)]
        static extern IntPtr ProcessImageCpp(
            byte[] img, 
            long data_len,
            ref ImageInfo imTemplate);

        public async Task<ImageProcessResponse> ProcessImage(ImageProcessRequest request)
        {
            var errors = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(request, new ValidationContext(request), errors, true);

            //if (!isValid)
            //{
            //    return new ImageProcessResponse
            //    {
            //        Image = null,
            //        IsSuccess = false,
            //        Message = errors.ToString()
            //    };
            //}

            byte[] imageData;
            byte[] result;
            int len;

            ImageInfo imInfo = new ImageInfo();
            MemoryStream convertedImageMemoryStream;

            using (var memoryStream = new MemoryStream())
            {
                await request.File.CopyToAsync(memoryStream);
                imageData = memoryStream.ToArray();
                result = new byte[imageData.Length];

                IntPtr ptr = ProcessImageCpp(
                                imageData, 
                                memoryStream.Length,
                                ref imInfo);

                byte[] imagePixels = new byte[imInfo.size];
                Marshal.Copy(imInfo.data, imagePixels, 0, imInfo.size);
                if (imInfo.data != IntPtr.Zero)
                {
                    ReleaseMemoryFromC(imInfo.data);
                }
                convertedImageMemoryStream = new MemoryStream(imagePixels);

                System.Drawing.Image processed = new Bitmap(convertedImageMemoryStream);

                processed.Save("C:/Users/SillySharp/Desktop/outputCSharp.png");

                return null;

                //byte[] bytes = new byte[len];
                //Marshal.Copy(ptr, bytes, 0, len);

                //using var stream = new MemoryStream(bytes);
                //IFormFile file = new FormFile(
                //    stream,
                //    0,
                //    bytes.Length,
                //    "Data",
                //    "fileName.png")
                //{
                //    Headers = new HeaderDictionary()
                //};

                //var img = Bitmap.FromHbitmap(ptr);

                //img.Save("C:/Users/SillySharp/Desktop/outputCSharp.png");

                //return new ImageProcessResponse
                //{
                //    Image = file,
                //    IsSuccess = true,
                //    Message = string.Empty,
                //    bytes = bytes
                //}; ;
                ////using (var fileStream = new FileStream("C:/Users/SillySharp/Desktop/outputCSharp.png", FileMode.Create))
                ////{
                ////    await fileStream.WriteAsync(bytes, 0, bytes.Length);
                ////}
            }

            //return null;
        }
    }
}
