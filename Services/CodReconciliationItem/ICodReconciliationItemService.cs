
using Veltrix.DTOs.CodReconciliationDTO;

namespace Veltrix.Services.CodReconciliationItem
{
    public interface ICodReconciliationItemService
    {
        Task<CodReconciliationItemDto?> GetByIdAsync(int id);
        Task<IReadOnlyList<CodReconciliationItemDto>> GetByReconciliationIdAsync(int reconciliationId);
        Task<IReadOnlyList<CodReconciliationItemDto>> GetByOrderIdAsync(int orderId);

    }
}
