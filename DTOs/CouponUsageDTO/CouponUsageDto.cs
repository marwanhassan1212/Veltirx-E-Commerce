namespace Veltrix.DTOs.CouponUsageDTO
{
    public class CouponUsageDto
    {
        public long Id { get; set; }

        public int CouponId { get; set; }

        public string CouponCode { get; set; } = string.Empty;

        public decimal DiscountAmount { get; set; }

        public DateTime UsedAt { get; set; }

        public string UsedByUserId { get; set; } = string.Empty;

        public string UsedByUserName { get; set; } = string.Empty;

        public int OrderId { get; set; }
    }
}
