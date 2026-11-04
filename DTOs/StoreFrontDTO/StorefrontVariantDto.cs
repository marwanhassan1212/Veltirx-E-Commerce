namespace Veltrix.DTOs.Storefront
{
    public class StorefrontVariantDto
    {
        public int Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsDefault { get; set; }
        public List<int> OptionValueIds { get; set; } = new();
        public List<StorefrontProductImageDto> Images { get; set; } = new();
    }
}