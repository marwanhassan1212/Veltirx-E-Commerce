namespace Veltrix.DTOs.Storefront
{
    public class StorefrontProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string SKU { get; set; } = string.Empty;

        public List<StorefrontProductImageDto> Images { get; set; } = new();

        public List<StorefrontVariantDto> Variants { get; set; } = new();

        public List<StorefrontOptionDto> Options { get; set; } = new();
    }
}