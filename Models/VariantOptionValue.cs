namespace Veltrix.Models
{
    public class VariantOptionValue
    {
        public int Id { get; set; }

        public string Value { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public VariantOptions VariantOption { get; set; } = null!;
        public int VariantOptionId { get; set; }
        public ICollection<ProductVariantOption> ProductVariantOptions { get; set; } = new List<ProductVariantOption>();

    }
}