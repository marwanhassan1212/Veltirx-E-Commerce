using System.ComponentModel.DataAnnotations;

namespace Veltrix.DTOs.ProductImageDTO
{
    public class UpdateProductImageDto
    {
        [Required]
        [Url]
        [StringLength(2048)]
        public string ImageUrl { get; set; } = string.Empty;

        [StringLength(300)]
        public string? AltText { get; set; }

        [Range(0, int.MaxValue)]
        public int DisplayOrder { get; set; }
    }
}
