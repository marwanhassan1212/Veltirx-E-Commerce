using Veltrix.DTOs.InventoryDTO;
using Veltrix.Models;
namespace Veltrix.Repositories.Inventory
{
    public interface IInventoryRepository
    {
        Task<Models.Inventory?> GetByIdAsync(int Id);
        Task<Models.Inventory?> GetByProductVariantIdAsync(int productId);
        Task<IReadOnlyList<Models.InventoryTransaction>> GetTransactionsByInventoryIdAsync(int inventoryId);
        Task<int?> TryDecreaseStockAsync(int inventoryId, int quantity);
        Task<(int QuantityBefore, int QuantityAfter)?> TryIncreaseStockAsync(int inventoryId, int quantity);
        void Update(Models.Inventory inventory);
        Task AddAsync(Models.Inventory inventory);
        Task<IReadOnlyList<Models.Inventory>> GetAllAsync();
        Task<(IReadOnlyList<Models.Inventory> Items, int TotalCount)> GetFilteredAsync(InventoryFilterDto filter);

    }
}
