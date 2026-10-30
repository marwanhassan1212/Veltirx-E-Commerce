
using Microsoft.EntityFrameworkCore;
using Veltrix.Data;

namespace Veltrix.Repositories.CouponUsage
{
    public class CouponUsageRepository : ICouponUsageRepository
    {
        private readonly AppDbContext _db;
        public CouponUsageRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Models.CouponUsage couponUsage)
        {
            await _db.CouponUsages.AddAsync(couponUsage);
        }

        public async Task<IReadOnlyList<Models.CouponUsage>> GetByCouponIdAsync(int couponId)
        {
            return await _db.CouponUsages.AsNoTracking()
                .Where(x => x.CouponId == couponId)
                .OrderByDescending(x => x.UsedAt)
                .ToListAsync();
        }

        public async Task<Models.CouponUsage?> GetByOrderIdAsync(int orderId)
        {
            return await _db.CouponUsages.FirstOrDefaultAsync(x => x.OrderId == orderId);
        }

        public async Task<IReadOnlyList<Models.CouponUsage>> GetByUserIdAsync(string userId)
        {
            return await _db.CouponUsages.AsNoTracking()
                .Where(x => x.UsedByUserId == userId)
                .OrderByDescending(x => x.UsedAt)
                .ToListAsync();
                

        }
    }
}
