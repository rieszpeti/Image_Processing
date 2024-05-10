using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.REPR;

namespace Application.Services
{
    public class ImageProcessingService : IImageProcessingService
    {
        [StructLayout(LayoutKind.Sequential)]
        class MyStruct
        {
            public int a;
            public char b;
        }

        private const string dllPath = @"..\..\..\..\x64\Debug\Application.CPP.dll";
        //private const string dllPath = @"C:\Users\SillySharp\source\repos\Image_Processing\x64\Debug\Application.CPP.dll";

        //[DllImport("Application.CPP.dll", EntryPoint = "DoSomethingInC")]
        [DllImport(dllPath)]
        public static extern int DoSomethingInC(ushort ExampleParam, char AnotherExampleParam);

        [DllImport(dllPath)]
        //[DllImport("Application.CPP.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ProcessImageCPP(string base64Image);

        [DllImport(dllPath)]
        public static extern void GetString(StringBuilder buffer);

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
            ushort var1 = 2;
            char var2 = 'a';
            var valami = DoSomethingInC(var1, var2);

            ProcessImageCPP(request.Image);

            StringBuilder buffer = new StringBuilder();
            GetString(buffer);


            return null;
        }
    }
}
