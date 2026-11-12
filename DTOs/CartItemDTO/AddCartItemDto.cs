using System.ComponentModel.DataAnnotations;

namespace Veltrix.DTOs.CartItemDTO
{
    public class AddCartItemDto
    {
        [Range(1 , int.MaxValue)]
        public int ProductVariantId { get; set; }

        [Range(1 , int.MaxValue)]
        public int Quantity { get; set; }
    }
}
