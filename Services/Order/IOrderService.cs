
using Veltrix.DTOs.OrderDTO;

namespace Veltrix.Services.Order
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(string userId, CreateOrderDto dto);
    }
}
