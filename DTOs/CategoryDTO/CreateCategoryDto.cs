using System.ComponentModel.DataAnnotations;

namespace Veltrix.DTOs.CategoryDTO
{
    public class CreateCategoryDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(180 , MinimumLength = 1)]
        public string Slug { get; set; } = string.Empty;
        [StringLength(2048)]
        public string? ImageUrl { get; set; } = string.Empty;
        [StringLength(1000)]
        public string? Description { get; set; } = string.Empty;
        [Range(0, int.MaxValue)]
        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

    }
}
