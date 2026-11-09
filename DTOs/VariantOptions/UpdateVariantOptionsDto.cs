using System.ComponentModel.DataAnnotations;

namespace Veltrix.DTOs.VariantOptionsDTO
{
    public class UpdateVariantOptionsDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
    }
}