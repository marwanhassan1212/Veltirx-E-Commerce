
using Microsoft.EntityFrameworkCore;
using Veltrix.Data;

namespace Veltrix.Repositories.CodReconciliationItem
{
    public class CodReconciliationItemRepository : ICodReconciliationItemRepository
    {
        private readonly AppDbContext _db;
        public CodReconciliationItemRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Models.CodReconciliationItem item)
        {
            await _db.CodReconciliationItems.AddAsync(item);
        }

        public void Delete(Models.CodReconciliationItem item)
        {
            _db.CodReconciliationItems.Remove(item);
        }

        public async Task<Models.CodReconciliationItem?> GetByIdAsync(int id)
        {
            return await _db.CodReconciliationItems.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<Models.CodReconciliationItem>> GetByOrderIdAsync(int orderId)
        {
            return await _db.CodReconciliationItems
                .Where(x => x.OrderId == orderId)
                .AsNoTracking()
                .Include(x => x.Reconciliation)
                .OrderByDescending(x => x.Reconciliation.CreatedAt)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Models.CodReconciliationItem>> GetByReconciliationIdAsync(int reconciliationId)
        {
            return await _db.CodReconciliationItems
               .Where(x => x.ReconciliationId == reconciliationId)
               .AsNoTracking()
               .Include(x => x.Order)
               .OrderBy(x => x.Id)
               .ToListAsync();
        }

        public void Update(Models.CodReconciliationItem item)
        {
            _db.CodReconciliationItems.Update(item);
        }
    }
}
