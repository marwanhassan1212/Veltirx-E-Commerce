using Veltrix.Models;

namespace Veltrix.DTOs.CartDTO
{
    public class CartDTO
    {
        public int Id { get; set; }
        public List<Models.CartItem> Items { get; set; } = new();
        public int TotalItems { get; set; }
        public decimal SubTotal { get; set; }
    }
}
