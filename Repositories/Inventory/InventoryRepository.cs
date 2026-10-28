using Microsoft.EntityFrameworkCore;
using Veltrix.Data;
using Veltrix.Models;

namespace Veltrix.Repositories.Inventory
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _db;
        public InventoryRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Models.Inventory inventory)
        {
            await _db.Inventories.AddAsync(inventory);
        }

        public async Task<Models.Inventory?> GetByIdAsync(int Id)
        {
            return await _db.Inventories.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<Models.Inventory?> GetByProductIdAsync(int productId)
        {
            return await _db.Inventories.FirstOrDefaultAsync(x => x.ProductId == productId);
        }

        public async Task<IReadOnlyList<Models.InventoryTransaction>> GetTransactionsByInventoryIdAsync(int inventoryId)
        {
            return await _db.InventoryTransactions.Where(x => x.InventoryId == inventoryId)
                 .AsNoTracking().OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        public void Update(Models.Inventory inventory)
        {
            _db.Inventories.Update(inventory);
        }
    }
}
