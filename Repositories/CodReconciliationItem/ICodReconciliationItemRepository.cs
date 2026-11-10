using Veltrix.Models;

namespace Veltrix.Repositories.CodReconciliationItem
{
    public interface ICodReconciliationItemRepository
    {
        Task<Models.CodReconciliationItem?> GetByIdAsync(int id);
        Task<IReadOnlyList<Models.CodReconciliationItem>> GetByReconciliationIdAsync(int reconciliationId);
        Task<IReadOnlyList<Models.CodReconciliationItem>> GetByOrderIdAsync(int orderId);
        Task AddAsync(Models.CodReconciliationItem item);
        void Update(Models.CodReconciliationItem item);

        void Delete(Models.CodReconciliationItem item);
    }
}