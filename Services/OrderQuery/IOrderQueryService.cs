using Veltrix.DTOs.OrderDTO;
using Veltrix.Repositories.Order;
namespace Veltrix.Services.Order
{
    public interface IOrderQueryService
    {
        Task<OrderDto?> GetCustomerOrderByIdAsync(string userId, int orderId);
        Task<OrderDto?> GetCustomerOrderByNumberAsync(string userId, string orderNumber);
        Task<IReadOnlyList<OrderDto>> GetCustomerOrdersAsync(string userId);
        Task<(IReadOnlyList<OrderSummaryDto> Items, int TotalCount)> GetPagedOrdersAsync(OrderFilterParameters filters);

    }
}
