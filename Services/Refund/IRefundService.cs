
using Veltrix.DTOs.RefundDTO;
using Veltrix.Repositories.Refund;

namespace Veltrix.Services.Refund
{
    public interface IRefundService
    {
        Task<RefundDto?> GetByIdAsync(int id);
        Task<RefundDto?> GetByRefundNumberAsync(string refundNumber);
        Task<IReadOnlyList<RefundDto>> GetByReturnIdAsync(int returnId);
        Task<IReadOnlyList<RefundDto>> GetAllAsync();

        Task<(IReadOnlyList<RefundDto> Items, int TotalCount)> GetPagedAsync(RefundFilterParameters filters);


    }
}
