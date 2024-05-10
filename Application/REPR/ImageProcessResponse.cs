using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.REPR
{
    public class ImageProcessResponse
    {
        public FileStream? Image { get; init; }

        [Required]
        public required bool IsSuccess { get; init; }

        [Required]
        public required string Message { get; init; } = null!;
    }
}
