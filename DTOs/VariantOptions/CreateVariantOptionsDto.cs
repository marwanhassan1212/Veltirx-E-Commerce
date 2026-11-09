using System.ComponentModel.DataAnnotations;

namespace Veltrix.DTOs.VariantOptionsDTO
{
    public class CreateVariantOptionsDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }
    }
}