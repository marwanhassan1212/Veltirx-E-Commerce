namespace Veltrix.Models
{
    public class ProductVariantOption
    {
        public int Id { get; set; }
        public ProductVariant ProductVariant { get; set; } = null!;
        public int ProductVariantId { get; set; }
        public VariantOptionValue VariantOptionValue { get; set; } = null!;
        public int VariantOptionValueId { get; set; }
    }
}