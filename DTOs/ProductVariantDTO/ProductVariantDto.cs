namespace Veltrix.DTOs.ProductVariantDTO
{
    public class ProductVariantDto
    {
        public int Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public int ProductId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<int> VariantOptionValueIds { get; set; } = new();

        public List<ProductVariantOptionDto> SelectedOptions { get; set; } = new();
    }

    public class ProductVariantOptionDto
    {
        public string OptionName { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}