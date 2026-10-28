
using Microsoft.EntityFrameworkCore;
using Veltrix.Data;

namespace Veltrix.Repositories.Coupon
{
    public class CouponRepository : ICouponRepository
    {
        private readonly AppDbContext _db;
        public CouponRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Models.Coupon coupon)
        {
            await _db.Coupons.AddAsync(coupon);
        }

        public async Task<IReadOnlyCollection<Models.Coupon>> GetAllAsync()
        {
            return await _db.Coupons.AsNoTracking()
                .OrderByDescending(x => x.Id).ToListAsync();
        }

        public async Task<Models.Coupon?> GetByCodeAsync(string code)
        {
            return await _db.Coupons.FirstOrDefaultAsync(x => x.Code == code);
        }

        public async Task<Models.Coupon?> GetByIdAsync(int Id)
        {
            return await _db.Coupons.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public void Update(Models.Coupon coupon)
        {
            _db.Coupons.Update(coupon);
        }
    }
}
