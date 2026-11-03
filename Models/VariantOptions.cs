namespace Veltrix.Models
{
    public class VariantOptions
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public Product Product { get; set; } = null!;
        public int ProductId { get; set; }

        public ICollection<VariantOptionValue> Values { get; set; } = new List<VariantOptionValue>();

    }
}
