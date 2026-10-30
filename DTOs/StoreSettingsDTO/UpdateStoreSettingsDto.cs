using System.ComponentModel.DataAnnotations;

namespace Veltrix.DTOs.StoreSettingsDTO
{
    public class UpdateStoreSettingsDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string StoreName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? StoreDescription { get; set; }

        [Url]
        [StringLength(2048)]
        public string? LogoUrl { get; set; }

        [EmailAddress]
        [StringLength(256)]
        public string? SupportEmail { get; set; }

        [Phone]
        [StringLength(30)]
        public string? SupportPhone { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string CurrencyCode { get; set; } = "EGP";
        public bool IsStoreOpen { get; set; }
        public bool IsActive { get; set; }

        [StringLength(1000)]
        public string? MaintenanceMessage { get; set; }
    }
}
