using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.CSharp.ModelValidation;
using Application.Models;
using Microsoft.AspNetCore.Http;

namespace Application.REPR
{
    public record ImageProcessRequest
    {
        [Required(ErrorMessage = "Please select a file.")]
        [DataType(DataType.Upload)]
        [MaxFileSize(5 * 1024 * 1024)]
        [AllowedExtensions([".jpg", ".png"])]
        public required IFormFile File { get; init; }
    }
}
