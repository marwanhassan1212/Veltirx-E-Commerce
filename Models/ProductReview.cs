namespace Veltrix.Models
{
    public class ProductReview
    {
        public long Id { get; set; }

        public byte Rating { get; set; }
        public string? Title { get; set; }
        public string? Comment { get; set; }
        public bool IsApproved { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }




        public Product Product { get; set; } = null!;
        public int ProductId { get; set; }

        public ApplicationUser User { get; set; } = null!;
        public string UserId { get; set; } = string.Empty;

        public OrderItem OrderItem { get; set; } = null!;
        public int OrderItemId { get; set; }
    }
}
