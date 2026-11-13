
using Veltrix.DTOs.OrderDTO;

namespace Veltrix.Services.Order
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(string userId, CreateOrderDto dto);
        Task<OrderDto?> CancelOrderAsync(string userId, int orderId, CancelOrderDto dto);
        Task<OrderDto?> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto, string? changedByUserId = null);

    }
}
