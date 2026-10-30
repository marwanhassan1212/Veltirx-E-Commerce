using System.ComponentModel.DataAnnotations;

namespace Veltrix.DTOs.UserAddressDTO
{
    public class CreateUserAddressDto
    {
        [Required]
        [StringLength(150, MinimumLength = 2)]
        public string RecipientName { get; set; } = string.Empty;

        [Required]
        [StringLength(30, MinimumLength = 7)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(250, MinimumLength = 5)]
        public string AddressLine1 { get; set; } = string.Empty;

        [StringLength(250)]
        public string? AddressLine2 { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Governorate { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string City { get; set; } = string.Empty;

        [StringLength(20)]
        public string? PostalCode { get; set; }

        [StringLength(500)]
        public string? DeliveryInstructions { get; set; }

        public bool IsDefault { get; set; }
    }
}
