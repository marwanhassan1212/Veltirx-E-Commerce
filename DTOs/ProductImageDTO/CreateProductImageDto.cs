using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Veltrix.DTOs.ProductImageDTO
{
    public class CreateProductImageDto
    {
        [Required]
        public IFormFile Image { get; set; } = null!;

        [StringLength(300)]
        public string? AltText { get; set; }

        public bool IsPrimary { get; set; }

        [Range(0, int.MaxValue)]
        public int DisplayOrder { get; set; }
    }
}