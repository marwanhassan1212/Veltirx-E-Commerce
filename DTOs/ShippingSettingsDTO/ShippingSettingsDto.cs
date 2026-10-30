namespace Veltrix.DTOs.ShippingSettingsDTO
{
    public class ShippingSettingsDto
    {
        public int Id { get; set; }
        public decimal ShippingFee { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
