using System.ComponentModel.DataAnnotations;

namespace Veltrix.DTOs.CartItemDTO
{
    public class UpdateCartItemDTO
    {
        [Range(1, 100)]
        public int Quantity { get; set; }
    }
}
