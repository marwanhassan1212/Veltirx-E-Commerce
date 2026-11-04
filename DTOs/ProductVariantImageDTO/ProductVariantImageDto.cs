namespace Veltrix.DTOs.ProductVariantImage
{
    public class ProductVariantImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? AltText { get; set; }
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ProductVariantId { get; set; }
    }
}