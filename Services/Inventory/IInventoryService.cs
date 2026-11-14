using Veltrix.DTOs.CategoryDTO;
using Veltrix.DTOs.Common;
using Veltrix.DTOs.InventoryDTO;
using Veltrix.DTOs.InventoryTransactionDTO;

namespace Veltrix.Services.Inventory
{
    public interface IInventoryService
    {
        Task<IReadOnlyList<InventoryDto>> GetAllAsync();
        Task<InventoryDto?> GetByIdAsync(int id);
        Task<InventoryDto?> GetByProductVariantIdAsync(int productVariantId);
        Task<IReadOnlyList<InventoryTransactionDto>> GetTransactionsAsync(int inventoryId);
        Task<IReadOnlyList<InventoryTransactionDto>> GetTransactionsByOrderIdAsync(int orderId);
        Task<InventoryDto> UpdateStockAsync(int inventoryId, UpdateInventoryDto dto);
        Task<PagedResultDto<InventoryDto>> GetFilteredAsync(InventoryFilterDto filter);


    }
}
