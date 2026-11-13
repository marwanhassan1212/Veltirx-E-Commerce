
using Microsoft.EntityFrameworkCore;
using Veltrix.Data;

namespace Veltrix.Repositories.CodReconciliation
{
    public class CodReconciliationRepository : ICodReconciliationRepository
    {
        private readonly AppDbContext _db;
        public CodReconciliationRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Models.CodReconciliation reconciliation)
        {
            await _db.CodReconciliations.AddAsync(reconciliation);
        }

        public async Task<IReadOnlyList<Models.CodReconciliation>> GetAllAsync()
        {
            return await _db.CodReconciliations
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<Models.CodReconciliation?> GetByIdAsync(int id)
        {
            return await _db.CodReconciliations
                 .Include(x => x.Items)
                     .ThenInclude(x => x.Order)
                 .Include(x => x.ReconciledByUser)
                 .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Models.CodReconciliation?> GetByReconciliationNumberAsync(string reconciliationNumber)
        {
            return await _db.CodReconciliations
                 .Include(x => x.Items)
                     .ThenInclude(x => x.Order)
                 .Include(x => x.ReconciledByUser)
                 .FirstOrDefaultAsync(
                     x => x.ReconciliationNumber == reconciliationNumber);
        }

        public void Update(Models.CodReconciliation reconciliation)
        {
            _db.CodReconciliations.Update(reconciliation);
        }

        public async Task<(IReadOnlyList<Models.CodReconciliation> Items, int TotalCount)> GetPagedAsync(CodReconciliationFilterParameters filters)
        {
            if (filters == null)
                throw new ArgumentNullException(nameof(filters));

            var pageNumber = Math.Max(1, filters.PageNumber);
            var pageSize = Math.Clamp(filters.PageSize, 1, 100);

            var query = _db.CodReconciliations
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filters.ReconciliationNumber))
            {
                var number = filters.ReconciliationNumber.Trim();

                query = query.Where(x =>
                    x.ReconciliationNumber.Contains(number));
            }

            if (filters.Status.HasValue)
            {
                query = query.Where(x => x.Status == filters.Status.Value);

            }

            if (!string.IsNullOrWhiteSpace(filters.ReconciledByUserId))
            {
                query = query.Where(x => x.ReconciledByUserId == filters.ReconciledByUserId);
            }

            if (filters.FromDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt >= filters.FromDate.Value.Date);

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
