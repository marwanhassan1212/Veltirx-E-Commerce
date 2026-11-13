
using Veltrix.Enums;

namespace Veltrix.DTOs.CouponDTO
{
    public class CouponListDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public int UsageCount { get; set; }
        public int? UsageLimit { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; }

        public bool IsExpired => ExpiresAt < DateTime.UtcNow;

        public bool IsUpcoming => StartsAt > DateTime.UtcNow;

        public bool IsUsageLimitReached =>
            UsageLimit.HasValue && UsageCount >= UsageLimit.Value;
    }
}
