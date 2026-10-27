namespace Veltrix.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
        public Order Order { get; set; } = null!;
        public int OrderId { get; set; }
        public Product Product { get; set; } = null!;
        public int ProductId { get; set; }

        public ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

    }
}
