using System.ComponentModel.DataAnnotations;

namespace Veltrix.DTOs.OrderDTO
{
    public class CreateOrderDto
    {
        [Required]
        [StringLength(150, MinimumLength = 2)]
        public string ShippingRecipientName { get; set; } = string.Empty;

        [Required]
        [StringLength(30, MinimumLength = 7)]
        public string ShippingPhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(250, MinimumLength = 5)]
        public string ShippingAddressLine1 { get; set; } = string.Empty;

        [StringLength(250)]
        public string? ShippingAddressLine2 { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string ShippingCity { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string ShippingGovernorate { get; set; } = string.Empty;

        [StringLength(20)]
        public string? ShippingPostalCode { get; set; }

        [StringLength(1000)]
        public string? ShippingInstructions { get; set; }

        [StringLength(50)]
        public string? CouponCode { get; set; }
    }
}
