namespace Veltrix.Models
{
    public class ProductVariant
    {
        public int Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsDefault { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }


        public Product Product { get; set; } = null!;
        public int ProductId { get; set; }
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<ProductVariantOption> VariantOptions { get; set; } = new List<ProductVariantOption>();
        public ICollection<ProductVariantImage> Images { get; set; } = new List<ProductVariantImage>();


        public Inventory? Inventory { get; set; }
    }
}
