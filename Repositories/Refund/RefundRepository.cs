
using Microsoft.EntityFrameworkCore;
using Veltrix.Data;

namespace Veltrix.Repositories.Refund
{
    public class RefundRepository : IRefundRepository
    {
        private AppDbContext _db;
        public RefundRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Models.Refund refund)
        {
            await _db.Refunds.AddAsync(refund);
        }

        public async Task<IReadOnlyList<Models.Refund>> GetAllAsync()
        {
            return await _db.Refunds.AsNoTracking()
                .OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        public async Task<Models.Refund?> GetByIdAsync(int id)
        {
            return await _db.Refunds.Include(x => x.Return)
                .Include(x => x.ProcessedByUserId)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Models.Refund?> GetByRefundNumberAsync(string refundNumber)
        {
            return await _db.Refunds
               .Include(x => x.Return)
               .Include(x => x.ProcessedByUser)
               .FirstOrDefaultAsync(x => x.RefundNumber == refundNumber);
        }

        public async Task<IReadOnlyList<Models.Refund>> GetByReturnIdAsync(int returnId)
        {
            return await _db.Refunds
              .Where(x => x.ReturnId == returnId)
              .AsNoTracking()
              .Include(x => x.ProcessedByUser)
              .OrderByDescending(x => x.CreatedAt)
              .ToListAsync();
        }

        public void Update(Models.Refund refund)
        {
            _db.Refunds.Update(refund);
        }
    }
}
