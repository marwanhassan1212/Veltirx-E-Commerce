using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Veltrix.DTOs.ProductVariantImage
{
    public class UpdateProductVariantImageDto
    {
        public IFormFile? Image { get; set; }

        [MaxLength(300)]
        public string? AltText { get; set; }
        public int DisplayOrder { get; set; }
    }
}