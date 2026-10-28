using Microsoft.EntityFrameworkCore;
using Veltrix.Data;

namespace Veltrix.Repositories.InventoryTransaction
{
    public class InventoryTransactionRepository : IInventoryTransactionRepository
    {
        private readonly AppDbContext _db;
        public InventoryTransactionRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Models.InventoryTransaction transaction)
        {
            await _db.InventoryTransactions.AddAsync(transaction);
        }

        public async Task<Models.InventoryTransaction?> GetByIdAsync(long id)
        {
            return await _db.InventoryTransactions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<Models.InventoryTransaction>> GetByInventoryIdAsync(int inventoryId)
        {
            return await _db.InventoryTransactions.Where(x => x.InventoryId == inventoryId)
                .AsNoTracking().OrderByDescending(x => x.CreatedAt).
                ThenByDescending(x => x.Id).ToListAsync();
        }

        public async Task<IReadOnlyList<Models.InventoryTransaction>> GetByOrderIdAsync(int orderId)
        {
            return await _db.InventoryTransactions
              .AsNoTracking()
              .Where(x => x.OrderId == orderId)
              .OrderByDescending(x => x.CreatedAt)
              .ThenByDescending(x => x.Id)
              .ToListAsync();
        }
    }
}
