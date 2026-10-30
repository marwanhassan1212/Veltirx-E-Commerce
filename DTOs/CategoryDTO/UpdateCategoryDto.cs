using System.ComponentModel.DataAnnotations;

namespace Veltrix.DTOs.CategoryDTO
{
    public class UpdateCategoryDto
    {
        [Required]
        [StringLength(150, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(180, MinimumLength = 1)]
        public string Slug { get; set; } = string.Empty;

        [StringLength(2048)]
        public string? ImageUrl { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Range(0, int.MaxValue)]
        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
