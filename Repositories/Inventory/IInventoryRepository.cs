using Veltrix.Models;
namespace Veltrix.Repositories.Inventory
{
    public interface IInventoryRepository
    {
        Task<Models.Inventory?> GetByIdAsync(int Id);
        Task<Models.Inventory?> GetByProductIdAsync(int productId);
        Task<IReadOnlyList<Models.InventoryTransaction>> GetTransactionsByInventoryIdAsync(int inventoryId);
        Task<int?> TryDecreaseStockAsync(int inventoryId, int quantity);
        Task<(int QuantityBefore, int QuantityAfter)?> TryIncreaseStockAsync(int inventoryId, int quantity);
        void Update(Models.Inventory inventory);
        Task AddAsync(Models.Inventory inventory);
    }
}
