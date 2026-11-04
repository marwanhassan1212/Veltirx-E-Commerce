using System.ComponentModel.DataAnnotations;

namespace Veltrix.DTOs.VariantOptionValueDTO
{
    public class CreateVariantOptionValueDto
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Value { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int DisplayOrder { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int VariantOptionId { get; set; }
    }
}