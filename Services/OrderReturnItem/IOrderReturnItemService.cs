
using Veltrix.DTOs.OrderReturnDTO;

namespace Veltrix.Services.OrderReturnItem
{
    public interface IOrderReturnItemService
    {
        Task<OrderReturnItemDto?> GetByIdAsync(int id);
        Task<IReadOnlyList<OrderReturnItemDto>> GetByReturnIdAsync(int returnId);
        Task<IReadOnlyList<OrderReturnItemDto>> GetByOrderItemIdAsync(int orderItemId);

    }
}
