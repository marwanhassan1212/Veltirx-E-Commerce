
using System.ComponentModel.DataAnnotations;
using Veltrix.Enums;

namespace Veltrix.DTOs.CouponDTO
{
    public class CouponDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        [RegularExpression(@"^[A-Za-z0-9_-]+$",
            ErrorMessage = "Coupon code can contain only letters, numbers, underscores and hyphens.")]
        public string Code { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [EnumDataType(typeof(DiscountType))]
        public DiscountType DiscountType { get; set; }

        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal DiscountValue { get; set; }

        [Range(typeof(decimal), "0", "999999999")]
        public decimal? MinimumOrderAmount { get; set; }

        [Range(typeof(decimal), "0", "999999999")]
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
