using System.ComponentModel.DataAnnotations;

namespace Veltrix.DTOs.ProductReviewDTO
{
    public class UpdateProductReviewDto
    {
        [Required]
        [Range(1, 5)]
        public byte Rating { get; set; }

        [StringLength(200)]
        public string? Title { get; set; }

        [StringLength(2000)]
        public string? Comment { get; set; }
    }
}
