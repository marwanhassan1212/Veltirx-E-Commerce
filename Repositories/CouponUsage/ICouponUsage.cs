namespace Veltrix.Repositories.CouponUsage
{
    public interface ICouponUsage
    {
        Task<IReadOnlyList<Models.CouponUsage>> GetByCouponIdAsync(int couponId);
        Task<IReadOnlyList<Models.CouponUsage>> GetByUserIdAsync(string userId);
        Task<Models.CouponUsage?> GetByOrderIdAsync(int orderId);
        Task AddAsync(Models.CouponUsage couponUsage);
    }
}
