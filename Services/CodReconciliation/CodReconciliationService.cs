
using Veltrix.DTOs.CodReconciliationDTO;
using Veltrix.Repositories.CodReconciliation;
using Veltrix.UnitOfWork;

namespace Veltrix.Services.CodReconciliation
{
    public class CodReconciliationService : ICodReconciliationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CodReconciliationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CodReconciliationDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                return null;

            var reconciliation = await _unitOfWork.CodReconciliations.GetByIdAsync(id);
            if (reconciliation == null)
                return null;

            return MapToDto(reconciliation);
        }

        public async Task<CodReconciliationDto?> GetByReconciliationNumberAsync(string reconciliationNumber)

        {
            if (string.IsNullOrWhiteSpace(reconciliationNumber))
                return null;

            var reconciliation = await _unitOfWork.CodReconciliations
                    .GetByReconciliationNumberAsync(reconciliationNumber);

            if (reconciliation == null)
                return null;
            return MapToDto(reconciliation);
        }

        public async Task<IReadOnlyList<CodReconciliationDto>> GetAllAsync()
        {
            var reconciliations = await _unitOfWork.CodReconciliations.GetAllAsync();
            return reconciliations
                .Select(MapToDto)
                .ToList();
        }

        private static CodReconciliationDto MapToDto(Models.CodReconciliation reconciliation)
        {
            return new CodReconciliationDto
            {
                Id = reconciliation.Id,

                ReconciliationNumber = reconciliation.ReconciliationNumber,
                ExpectedAmount = reconciliation.ExpectedAmount,
                CollectedAmount = reconciliation.CollectedAmount,
                DifferenceAmount = reconciliation.DifferenceAmount,
                Status = reconciliation.Status,
                Notes = reconciliation.Notes,
                CreatedAt = reconciliation.CreatedAt,
                ReconciledAt = reconciliation.ReconciledAt,
                ReconciledByUserId = reconciliation.ReconciledByUserId,
                Items = reconciliation.Items
                    .Select(item => new CodReconciliationItemDto
                    {
                        Id = item.Id,

                        OrderId = item.OrderId,
                        ExpectedAmount = item.ExpectedAmount,
                        CollectedAmount = item.CollectedAmount,
                        DifferenceAmount = item.DifferenceAmount
                    })
                    .ToList()
            };

        }

        public async Task<(IReadOnlyList<CodReconciliationDto> Items, int TotalCount)> GetPagedAsync(CodReconciliationFilterParameters filters)
        {
            if (filters == null)
                throw new ArgumentNullException(nameof(filters));

            if (filters.FromDate.HasValue &&
                filters.ToDate.HasValue &&
                filters.FromDate.Value.Date > filters.ToDate.Value.Date)
            {
                throw new ArgumentException(
                    "FromDate cannot be later than ToDate.");
            }

            filters.PageNumber = Math.Max(1, filters.PageNumber);
            filters.PageSize = Math.Clamp(filters.PageSize, 1, 100);

            var result = await _unitOfWork.CodReconciliations
                .GetPagedAsync(filters);

            var items = result.Items
                .Select(MapToDto)
                .ToList();

            return (items, result.TotalCount);
        }

    }
}
