using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Veltrix.DTOs.ProductVariantImage
{
    public class CreateProductVariantImageDto
    {
        [Required]
        public IFormFile Image { get; set; } = null!;

        [MaxLength(300)]
        public string? AltText { get; set; }
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }
    }
}