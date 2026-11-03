using System.ComponentModel.DataAnnotations;

namespace Veltrix.DTOs.ProductVariantDTO
{
    public class SetVariantOptionsDto
    {
        [Required]
        [MinLength(1)]
        public List<int> VariantOptionValueIds { get; set; } = new();
    }
}