
using Veltrix.DTOs.CodReconciliationDTO;
using Veltrix.Repositories.CodReconciliation;

namespace Veltrix.Services.CodReconciliation
{
    public interface ICodReconciliationService
    {
        Task<CodReconciliationDto?> GetByIdAsync(int id);
        Task<CodReconciliationDto?> GetByReconciliationNumberAsync(string reconciliationNumber);
        Task<IReadOnlyList<CodReconciliationDto>> GetAllAsync();

        Task<(IReadOnlyList<CodReconciliationDto> Items, int TotalCount)> GetPagedAsync(CodReconciliationFilterParameters filters);


    }
}
