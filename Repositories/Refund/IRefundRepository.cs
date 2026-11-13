using Veltrix.Models;

namespace Veltrix.Repositories.Refund
{
    public interface IRefundRepository
    {
        Task<Models.Refund?> GetByIdAsync(int id);
        Task<Models.Refund?> GetByRefundNumberAsync(string refundNumber);
        Task<IReadOnlyList<Models.Refund>> GetByReturnIdAsync(int returnId);
        Task<IReadOnlyList<Models.Refund>> GetAllAsync();
        Task AddAsync(Models.Refund refund);
        void Update(Models.Refund refund);
        Task<(IReadOnlyList<Models.Refund> Items, int TotalCount)> GetPagedAsync(RefundFilterParameters filters);
    }
}