namespace Veltrix.Models
{
    public class CouponUsage
    {
        public long Id { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime UsedAt { get; set; }
        public Coupon Coupon { get; set; } = null!;
        public int CouponId { get; set; }
        public ApplicationUser UsedBy { get; set; } = null!;
        public string UsedByUserId { get; set; } = string.Empty;
        public Order Order { get; set; } = null!;
        public int OrderId { get; set; }



    }
}
