namespace Veltrix.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string VariantName { get; set; } = string.Empty;
        public string? VariantOptionsSnapshot { get; set; }

        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
        public int CancelledQuantity { get; set; }
        public int FulfilledQuantity { get; set; }
        public Order Order { get; set; } = null!;
        public int OrderId { get; set; }

        public ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

        public ProductVariant ProductVariant { get; set; } = null!;
        public int ProductVariantId { get; set; }

    }
}
