using System.ComponentModel.DataAnnotations;

namespace Veltrix.DTOs.ProductDTO
{
    public class UpdateProductDto
    {
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(220, MinimumLength = 1)]
        public string Slug { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ShortDescription { get; set; }

        [StringLength(4000)]
        public string? Description { get; set; }

        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }
    }
}
