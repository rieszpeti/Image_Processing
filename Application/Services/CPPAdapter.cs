using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.CSharp.Services
{
    internal static class CPPAdapter
    {
        public struct ImageInfo
        {
            public IntPtr data;
            public int size;
        }

        private const string dllPath = @"..\..\..\..\x64\Debug\Application.CPP.dll";

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ReleaseMemoryFromC(IntPtr buf);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ProcessImageCpp(
                                            byte[] img,
                                            long data_len,
                                            string fileExtension,
                                            ref ImageInfo imTemplate);
    }
}
