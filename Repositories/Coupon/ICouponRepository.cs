namespace Veltrix.Repositories.Coupon
{
    public interface ICouponRepository
    {
        Task<Models.Coupon?> GetByIdAsync(int Id);
        Task<Models.Coupon?> GetByCodeAsync(string code);
        Task<IReadOnlyCollection<Models.Coupon>> GetAllAsync();
        Task<bool> TryIncrementUsageCountAsync(int couponId);
        Task AddAsync(Models.Coupon coupon);
        void Update(Models.Coupon coupon);

    }
}
