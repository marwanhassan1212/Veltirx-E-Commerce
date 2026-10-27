namespace Veltrix.Models
{
    public class ShippingSettings
    {
        public int Id { get; set; }
        public decimal ShippingFee { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime UpdatedAt { get; set; }



        public ApplicationUser? UpdatedByUser { get; set; } = null!;
        public string? UpdatedByUserId { get; set; }

    }
}
