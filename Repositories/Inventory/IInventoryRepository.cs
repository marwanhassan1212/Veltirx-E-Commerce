using Veltrix.Models;
namespace Veltrix.Repositories.Inventory
{
    public interface IInventoryRepository
    {
        Task<Models.Inventory?> GetByIdAsync(int Id);
        Task<Models.Inventory?> GetByProductIdAsync(int productId);
        Task<IReadOnlyList<Models.InventoryTransaction>> GetTransactionsByInventoryIdAsync(int inventoryId);
        void Update(Models.Inventory inventory);

        Task AddAsync(Models.Inventory inventory);
    }
}
