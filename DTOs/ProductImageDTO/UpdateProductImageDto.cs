using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Veltrix.DTOs.ProductImageDTO
{
    public class UpdateProductImageDto
    {
        public IFormFile? Image { get; set; }

        [StringLength(300)]
        public string? AltText { get; set; }

        [Range(0, int.MaxValue)]
        public int DisplayOrder { get; set; }
    }
}