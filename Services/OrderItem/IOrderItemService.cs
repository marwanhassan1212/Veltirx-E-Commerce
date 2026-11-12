
using Veltrix.DTOs.OrderDTO;
using Veltrix.DTOs.OrderItemDTO;
using Veltrix.Repositories.Order;

namespace Veltrix.Services.Order
{
    public interface IOrderItemService
    {

        Task<OrderItemDto?> GetByIdAsync(int id);
        Task<IReadOnlyList<OrderItemDto>> GetByOrderIdAsync(int orderId, string? userId = null);
        Task<IReadOnlyList<OrderItemDto>> GetByProductIdAsync(int productId);

    }
    }
