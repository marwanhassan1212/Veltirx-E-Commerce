using Veltrix.Models;
using Veltrix.DTOs.CartItemDTO;

namespace Veltrix.DTOs.CartDTO
{
    public class CartDTO
    {
        public int Id { get; set; }
        public List<CartItemDTO.CartItemDTO> Items { get; set; } = new();
        public int TotalItems { get; set; }
        public decimal SubTotal { get; set; }
    }
}
