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
        public async Task<int?> TryDecreaseStockAsync(int inventoryId, int quantity)
        {
            if (inventoryId <= 0 || quantity <= 0)
                return null;

            var affectedRows = await _db.Inventories
                .Where(x => x.Id == inventoryId &&
                            x.StockQuantity >= quantity)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(
                        x => x.StockQuantity,
                        x => x.StockQuantity - quantity)
                    .SetProperty(
                        x => x.UpdatedAt,
                        DateTime.UtcNow));

            if (affectedRows == 0)
                return null;

            return await _db.Inventories
                .AsNoTracking()
                .Where(x => x.Id == inventoryId)
                .Select(x => (int?)x.StockQuantity)
                .SingleAsync();
        }
        public async Task<Models.Inventory?> GetByProductIdAsync(int productvariantId)
        {
            return await _db.Inventories.FirstOrDefaultAsync(x => x.ProductVariantId == productvariantId);
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

        public async Task<(int QuantityBefore, int QuantityAfter)?> TryIncreaseStockAsync(int inventoryId, int quantity)
        {
            if (inventoryId <= 0 || quantity <= 0)
                return null;

            var inventory = await _db.Inventories
                .Where(x => x.Id == inventoryId)
                .Select(x => new
                {
                    x.Id,
                    x.StockQuantity
                })
                .SingleOrDefaultAsync();

            if (inventory == null ||
                inventory.StockQuantity > int.MaxValue - quantity)
            {
                return null;
            }

            var quantityBefore = inventory.StockQuantity;

            var affectedRows = await _db.Inventories
                .Where(x =>
                    x.Id == inventoryId &&
                    x.StockQuantity == quantityBefore &&
                    x.StockQuantity <= int.MaxValue - quantity)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(
                        x => x.StockQuantity,
                        x => x.StockQuantity + quantity)
                    .SetProperty(
                        x => x.UpdatedAt,
                        DateTime.UtcNow));

            if (affectedRows == 0)
                return null;

            return (quantityBefore, quantityBefore + quantity);
        }
    }
}
