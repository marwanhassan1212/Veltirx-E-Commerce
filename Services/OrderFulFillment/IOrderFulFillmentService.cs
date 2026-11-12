
using Veltrix.DTOs.OrderFulfillmentDTO;

namespace Veltrix.Services.OrderFulfillment
{
    public interface IOrderFulfillmentService
    {
        Task<OrderFulfillmentDto?> GetByIdAsync(int id);
        Task<OrderFulfillmentDto?> GetByFulfillmentNumberAsync(string fulfillmentNumber);
        Task<IReadOnlyList<OrderFulfillmentDto>> GetByOrderIdAsync(int orderId);
        Task<IReadOnlyList<OrderFulfillmentDto>> GetAllAsync();
    }
}
