using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CSharp.Models
{
    public class ImageProcessResponse
    {
        public required byte[] bytes { get; init; }

        public required string FileExtension { get; init; }
    }
}
