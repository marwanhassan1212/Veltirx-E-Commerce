using System.ComponentModel.DataAnnotations;
using Veltrix.Enums;

namespace Veltrix.DTOs.CouponDTO
{
    public class CreateCouponDto
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Code { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public DiscountType DiscountType { get; set; }

        [Range(typeof(decimal), "0.01", "9999999999999999.99")]
        public decimal DiscountValue { get; set; }

        [Range(typeof(decimal), "0", "9999999999999999.99")]
        public decimal? MinimumOrderAmount { get; set; }

        [Range(typeof(decimal), "0.01", "9999999999999999.99")]
        public decimal? MaximumDiscountAmount { get; set; }

        [Range(1, int.MaxValue)]
        public int? UsageLimit { get; set; }

        [Required]
        public DateTime StartsAt { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
