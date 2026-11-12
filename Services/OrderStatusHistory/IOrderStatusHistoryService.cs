using Veltrix.DTOs.OrderStatusDTO;

namespace Veltrix.Services.OrderStatusHistory
{
    public interface IOrderStatusHistoryService
    {
        Task<IReadOnlyList<OrderStatusHistoryDto>> GetByOrderIdAsync(int orderId);

    }
}
