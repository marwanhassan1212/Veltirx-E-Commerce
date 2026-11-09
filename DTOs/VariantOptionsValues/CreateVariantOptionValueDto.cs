using System.ComponentModel.DataAnnotations;

namespace Veltrix.DTOs.VariantOptionValueDTO
{
    public class CreateVariantOptionValueDto
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Value { get; set; } = string.Empty;


        [Required]
        [Range(1, int.MaxValue)]
        public int VariantOptionId { get; set; }
    }
}