using System.ComponentModel.DataAnnotations;

namespace Veltrix.DTOs.ProductVariantDTO
{
    public class CreateProductVariantDto
    {
        [Required]
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty;

        [Required]
        [Range(typeof(decimal), "0.01", "9999999999999999.99")]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }
    }
}
