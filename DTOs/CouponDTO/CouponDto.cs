using Veltrix.Enums;

namespace Veltrix.DTOs.CouponDTO
{
    public class CouponDto
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DiscountType DiscountType { get; set; }

        public decimal DiscountValue { get; set; }

        public decimal? MinimumOrderAmount { get; set; }

        public decimal? MaximumDiscountAmount { get; set; }

        public int? UsageLimit { get; set; }

        public int UsageCount { get; set; }

        public DateTime StartsAt { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
