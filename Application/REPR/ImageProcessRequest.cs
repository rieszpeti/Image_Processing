using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models;

namespace Application.REPR
{
    public record ImageProcessRequest
    {
        [Required]
        [Base64String]
        public required string Image { get; init; }

        [Required]
        [EnumDataType(typeof(EncodingType))]
        public required EncodingType EncodingType { get; init; }
    }
}
