namespace Veltrix.Repositories.InventoryTransaction
{
    public interface IInventoryTransactionRepository
    {
        Task<Models.InventoryTransaction?> GetByIdAsync(long id);
        Task<IReadOnlyList<Models.InventoryTransaction>> GetByInventoryIdAsync(int inventoryId);
        Task<IReadOnlyList<Models.InventoryTransaction>> GetByOrderIdAsync(int orderId);
        Task AddAsync(Models.InventoryTransaction transaction);
    }
}
