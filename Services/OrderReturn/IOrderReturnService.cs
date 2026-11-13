
using Veltrix.DTOs.OrderReturnDTO;
using Veltrix.Repositories.OrderReturn;

namespace Veltrix.Services.OrderReturn
{
    public interface IOrderReturnService
    {
        Task<OrderReturnDto?> GetByIdAsync(int id);
        Task<OrderReturnDto?> GetByReturnNumberAsync(string returnNumber);
        Task<IReadOnlyList<OrderReturnDto>> GetByOrderIdAsync(int orderId);
        Task<IReadOnlyList<OrderReturnDto>> GetAllAsync();
        Task<(IReadOnlyList<OrderReturnDto> Items, int TotalCount)> GetPagedAsync(OrderReturnFilterParameters filters);


    }

}
