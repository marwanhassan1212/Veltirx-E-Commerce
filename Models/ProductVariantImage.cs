namespace Veltrix.Models
{
    public class ProductVariantImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? AltText { get; set; }

        public bool IsPrimary { get; set; } = false;
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }

        public ProductVariant ProductVariant { get; set; } = null!;
        public int ProductVariantId { get; set; }
    }
}