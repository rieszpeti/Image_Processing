using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.CSharp.Services
{
    /// <summary>
    /// You can use only in debug mode
    /// </summary>
    internal static class CPPAdapter
    {
        public struct ImageInfo
        {
            public IntPtr data;
            public int size;
        }

#if DEBUG
        private const string DllPath = @"..\..\..\..\x64\Debug\Application.CPP.dll";
#else
        private const string DllPath = @"..\..\..\..\x64\Release\Application.CPP.dll";
#endif

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ReleaseMemoryFromC(IntPtr buf);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ProcessImageCpp(
                                            byte[] img,
                                            long data_len,
                                            string fileExtension,
                                            ref ImageInfo imTemplate);
    }
}
