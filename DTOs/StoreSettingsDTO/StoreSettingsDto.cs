namespace Veltrix.DTOs.StoreSettingsDTO
{
    public class StoreSettingsDto
    {
        public int Id { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public string? StoreDescription { get; set; }
        public string? LogoUrl { get; set; }
        public string? SupportEmail { get; set; }
        public string? SupportPhone { get; set; }
        public string CurrencyCode { get; set; } = "EGP";
        public bool IsStoreOpen { get; set; }
        public bool IsActive { get; set; }
        public string? MaintenanceMessage { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
