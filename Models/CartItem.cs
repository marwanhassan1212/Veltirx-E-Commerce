namespace Veltrix.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Cart Cart { get; set; } = null!;
        public int CartId { get; set; }
        public Product Product { get; set; } = null!;
        public int ProductId { get; set; }

    }
}
