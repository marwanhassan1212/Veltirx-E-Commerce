
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
                .Include(x => x.ProcessedByUser)
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


        public async Task<(IReadOnlyList<Models.Refund> Items, int TotalCount)>
            GetPagedAsync(RefundFilterParameters filters)
        {
            if (filters == null)
                throw new ArgumentNullException(nameof(filters));

            var pageNumber = Math.Max(1, filters.PageNumber);
            var pageSize = Math.Clamp(filters.PageSize, 1, 100);

            var query = _db.Refunds
                .AsNoTracking()
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(filters.RefundNumber))
            {
                var refundNumber = filters.RefundNumber.Trim();

                query = query.Where(x =>
                    x.RefundNumber.Contains(refundNumber));
            }

            // Filtering
            if (filters.ReturnId.HasValue)
            {
                query = query.Where(x =>
                    x.ReturnId == filters.ReturnId.Value);
            }

            if (filters.Status.HasValue)
            {
                query = query.Where(x =>
                    x.Status == filters.Status.Value);
            }

            if (!string.IsNullOrWhiteSpace(filters.ProcessedByUserId))
            {
                query = query.Where(x =>
                    x.ProcessedByUserId == filters.ProcessedByUserId);
            }

            if (filters.FromDate.HasValue)
            {
                query = query.Where(x =>
                    x.CreatedAt >= filters.FromDate.Value.Date);
            }

            if (filters.ToDate.HasValue)
            {
                var toDate = filters.ToDate.Value.Date.AddDays(1);

                query = query.Where(x =>
                    x.CreatedAt < toDate);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

    }
}
